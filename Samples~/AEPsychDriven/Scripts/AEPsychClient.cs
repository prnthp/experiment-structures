// Parts of this code is adapted from AEPsych
// https://github.com/facebookresearch/aepsych
// Lucy Owen, Jonathan Browder, Benjamin Letham, Gideon Stocek, Chase Tymms, Michael Shvartsman, 2021
// CC-BY-NC 4.0
// Please see https://github.com/facebookresearch/aepsych/blob/main/LICENSE

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TrialConfig = GenericDictionary<string, System.Collections.Generic.List<float>>;

public class AEPsychClient : MonoBehaviour
{
    public string serverAddress = "127.0.0.1";

    public int port = 5555;
    
    private static AEPsychClient _instance;

    public static AEPsychClient Instance
    {
        get
        {
            if (!FindObjectOfType<AEPsychClient>())
            {
                var go = new GameObject("AEPsychClient");
                _instance = go.AddComponent<AEPsychClient>();
                return _instance;
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(gameObject);
        else
            _instance = this;

        NetMQConfig.ThreadPoolSize = 2;
    }

    public class TrialMetaData
    {
        // TODO: Stub
    }
    
    public class AEPsychRequest
    {
        public enum Type
        {
            setup,
            ask,
            resume,
            tell,
            query
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public Type type { get; set; }
        public string version { get; set; } = "0.01";

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }
    }

    public class AePsychSetupRequest : AEPsychRequest
    {
        public class Message
        {
            public string config_str { get; set; }
        }
        public Message message { get; set; }
        
        public AePsychSetupRequest(string config)
        {
            type = Type.setup;
            message = new Message
            {
                config_str = config
            };
        }
    }

    public class AePsychAskRequest : AEPsychRequest
    {
        public string message { get; set; } = "";

        public AePsychAskRequest()
        {
            type = Type.ask;
        }
    }

    public class AePsychResumeRequest : AEPsychRequest
    {
        public class Message
        {
            public int strat_id { get; set; }
        }
        public Message message { get; set; }

        public AePsychResumeRequest(int stratId)
        {
            type = Type.resume;
            message = new Message
            {
                strat_id = stratId
            };
        }
    }

    public class AePsychTellRequest : AEPsychRequest
    {
        public class Message
        {
            public TrialConfig config;
            public int outcome { get; set; }
        }
        public Message message { get; set; }

        public AePsychTellRequest(TrialConfig trialConfig, int _outcome)
        {
            version = null; // version must not be present for some reason
            type = Type.tell;
            message = new Message
            {
                config = trialConfig,
                outcome = _outcome
            };
        }
    }

    public class AEPsychQuery : AEPsychRequest
    {
        public enum QueryType
        {
            min,
            max,
            prediction,
            inverse
        }

        public class Message
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public QueryType query_type { get; set; }
            public Dictionary<string, List<float>> x { get; set; }
            public float y { get; set; }
            public Dictionary<string, List<float>> constraints { get; set; }
            public bool probability_space { get; set; } = false;
        }
        public Message message;

        public AEPsychQuery(QueryType queryType)
        {
            version = null;
            type = Type.query;
            message = new Message
            {
                query_type = queryType
            };
        }
    }
    
    public class AEPsychExitRequest
    {
        public string type { get; set; } = "exit";
    }
    
    public class AEPsychTrialConfigResponse
    {
        public TrialConfig config { get; set; }
        public bool is_finished { get; set; }
    }

    public AEPsychRequest CurrentRequest { get; private set; }
    
    public enum State { Idle, Request, Requested }
    
    public State state { get; private set; }

    private RequestSocket _client;
    
    private void Start()
    {
        AsyncIO.ForceDotNet.Force();

        state = State.Idle;
    }
    
    private void Update()
    {
        switch (state)
        {
            case State.Idle:
            {
                break;
            }

            case State.Request:
            {
                StartCoroutine(RequestAEPsych(CurrentRequest));
                state = State.Requested;
                break;
            }

            case State.Requested:
            {
                break;
            }
        }
    }

    private Action<int> _setupCallback;
    private Action<TrialConfig, bool> _askCallback;
    private Action<int> _resumeCallback;
    private Action<string> _tellCallback;
    private Action<AEPsychQuery.Message> _queryCallback;
    
    public bool SetupTrials(string config, Action<int> callback)
    {
        _setupCallback = callback;
        return Request(new AePsychSetupRequest(config));
    }
    
    public bool AskForNextTrialConfig(Action<TrialConfig, bool> callback)
    {
        _askCallback = callback;
        return Request(new AePsychAskRequest());
    }
    
    public bool ResumeStrategy(int id, Action<int> callback)
    {
        _resumeCallback = callback;
        return Request(new AePsychResumeRequest(id));
    }

    public bool TellOutcome(AePsychTellRequest message, Action<string> callback)
    {
        _tellCallback = callback;
        return Request(message);
    }

    public bool Query(AEPsychQuery query, Action<AEPsychQuery.Message> callback)
    {
        _queryCallback = callback;
        return Request(query);
    }

    private bool Request(AEPsychRequest request)
    {
        if (state == State.Idle)
        {
            CurrentRequest = request;
            state = State.Request;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CancelRequest()
    {
        if (state == State.Requested)
        {
            StopAllCoroutines();
            return true;
        }
        else if (state == State.Idle)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator RequestAEPsych(AEPsychRequest req)
    {
        if (_client == null)
        {
            _client = new RequestSocket();
        }
        
        _client.Connect($"tcp://{serverAddress}:{port}");
        var request = req.ToJson();
        _client.SendFrame(request);
        // Debug.Log("Request: " + request);
        var response = "";
        while (!_client.TryReceiveFrameString(out response))
        {
            yield return null;
        }
        // Debug.Log("Response: " + response);
        switch (req.type)
        {
            case AEPsychRequest.Type.setup:
            {
                try
                {
                    _setupCallback.Invoke(JsonConvert.DeserializeObject<int>(response));
                }
                catch (JsonException e)
                {
                    Debug.LogError($"[AEPsychClient] Could not parse response {response} + \n ${e}");
                }

                break;
            }

            case AEPsychRequest.Type.ask:
            {
                try
                {
                    var trialConfigResponse = JsonConvert.DeserializeObject<AEPsychTrialConfigResponse>(response);
                    _askCallback.Invoke(trialConfigResponse.config, trialConfigResponse.is_finished);
                }
                catch (JsonException e)
                {
                    Debug.LogError($"[AEPsychClient] Could not parse response {response} + \n ${e}");
                }

                break;
            }

            case AEPsychRequest.Type.resume:
            {
                try
                {
                    _resumeCallback.Invoke(JsonConvert.DeserializeObject<int>(response));
                }
                catch (JsonException e)
                {
                    Debug.LogError($"[AEPsychClient] Could not parse response {response} + \n ${e}");
                }

                break;
            }

            case AEPsychRequest.Type.tell:
            {
                _tellCallback.Invoke(response);
                break;
            }

            case AEPsychRequest.Type.query:
            {
                try
                {
                    _queryCallback.Invoke(JsonConvert.DeserializeObject<AEPsychQuery.Message>(response));
                }
                catch (JsonException e)
                {
                    Debug.LogError($"[AEPsychClient] Could not parse response {response} + \n ${e}");
                }
                
                break;
            }
        }

        state = State.Idle;
    }

    public string RequestAEPsychBlocking(AEPsychRequest msg)
    {
        using (var client = new RequestSocket())
        {
            client.Connect($"tcp://{serverAddress}:{port}");
            client.SendFrame(msg.ToJson());
            var response = client.ReceiveFrameString();
            return response;
        }
    }

    private void OnDestroy()
    {
        if (_client != null)
        {
            _client.Close();
        }
    }
}
