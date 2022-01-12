# **Duplo!** - Experiment Structures for User Studies in Unity

[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

<!-- - [Basic Concepts](#basic-concepts)
- [Install](#install)
- [Configuration](#configuration) -->

<!-- toc -->

## Basic Concepts

Experiment Structures is simply a framework for a finite state machine that exploits Unity's Scene hierarchy for reordering the states and utilizes Unity's component Inspector for configuration. The framework has been used mainly for creating human behavioral experiments (psychophysics) and data collection applications.

The framework has 3 main components: `Block`, `Trial`, and `Phase`. A `Block` contains a series of `Trials`, a `Trial` contains a series of `Phases`. A `Trial` can have many repetitions and a `Phase` can have a finite duration. When put together, an experiment can look like this:

<img src="Images~/main_structures.svg" width=250px alt="Block→Trial→Phase diagram" />

While in the Unity Editor, the above structure would look like this:

<img src="Images~/main_structures_unity.png" width=250px alt="Structures in Unity Editor" />

Rearranging the Phases and Trials can be done by simply dragging the GameObjects around. Duplicating, copying and pasting is only a `Ctrl+D`, `Ctrl+C` and `Ctrl+V` away.

<hr />

If you are looking for a framework that does it all: UI, data logging, structured sessions, analysis, etc. - look at [UXF](https://github.com/immersivecognition/unity-experiment-framework) or [Psychopy](https://psychopy.org/)! (also, if you need millisecond accuracy, look away, Unity is not designed for that stuff!)

But if you're like me, who likes to implement stuff in their own way, this might be for you! This works best if you already have a "game" in Unity and want a way to control progression in a repeated fashion. In fact, I usually build my experiments as a "minigame" first and then tack on Experiment Structures afterwards to control the flow.

<hr />

<!-- TODO: gif -->

### Publications that have used earlier versions of Experiment Structures:

| Part of Publication | Publication (Conference/Journal) |
|---|---|
|User studies in VR, 2AFC psychophysics|**Chasm: A Screw Based Compact Haptic Actuator** (CHI 2020)|
|User studies in VR, data collection|**Haplets: Finger-Worn Wireless and Low-Encumbrance Vibrotactile Haptic Feedback for Virtual and Augmented Reality** (Frontiers in Virtual Reality, 2021)|

## Usage

Each component is designed to be a single component in a GameObject. All three components are `abstract` classes, meaning that the user must implement their own code with the requisite implementations for each component. This may seem tedious, but it is similar to how Unity's `MonoBehaviours` work. All components are, of course, re-usable. There are also a bunch utility and example Phases, Trials and Blocks readily available.

All classes are derived from `MonoBehaviour`, so serialization should work as expected and the structures should play well with Prefabs. Some Unity user callbacks (e.g. `FixedUpdate()`) are available.

### Phase

A Phase can be thought of as a GameObject that automatically starts, loops (think `Update()`) and stops itself. Phases can have a finite duration or loop forever.

Phases look like this:
```C#
public class MyPhase : Phase // Implement the base class
{
    public GameObject someObject;
    public Light someLight;
    public Trial someOtherTrial;

    public override void Enter()
    {
        GuaranteeUnityFrameCycle = true; // Make sure Update() is called

        someObject.SetActive(true);
    }

    public override void Loop()
    {
        // What you would normally put in Update() can go here
        if (Input.GetKeyUp(KeyCode.F)) { /* Pay Respects */ }

        if (Input.GetKeyUp(KeyCode.Q))
            trial.ExitTrial(); // End the trial prematurely and move on to the next trial

        if (Input.KonamiCodeEntered)
            trial.SkipToTrial(someOtherTrial); // End this trial and start "someOtherTrial" immediately.
    }

    public override void OnExit()
    {
        someLight.enabled = false;
    }
}
```

The inspector would show this:
<img src="Images~/myphase_example.png" width=400px alt="Example of a Phase" />

- Only On First Repetition: the Phase will only run through during the first repetition of the trial
- Duration: In seconds, how long should this Phase last. 
  - A negative number indicates that this Phase is to loop indefinitely. An event `NextPhase()`, called using `EventManager.Instance.NextPhase()` is used to exit the Phase.
  - Zero indicates that the Phase be run through once: `Enter()`→`Loop()`→`OnExit()`.
  - The property `GuaranteeUnityFrameCycle`, available only through code, can be set to `true` to ensure that a Phase with zero duration go through an entire Unity frame *at least* once. However, this does not mean that Unity's order of execution is guaranteed, nor is a single frame or a single `Loop()` guaranteed.
- Setting the GameObject holding the Phase to inactive (top-left tickbox in Inspector) will disable the Phase when the Scene is started.

### Trial

A Trial is like a list of Phases. It goes through each child Phase one-by-one according to the order in the hierarchy. A Trial typically has a number of repetitions (minimum is one). An endlessly repeating Trial can also be marked as `Endless` through the property accessible by code.

Trials look like this:
```C#
public class MyTrial : Trial
{
    public Text someText;
    public Canvas someCanvas;
    
    protected override void OnTrialBegin() // Optional override
    {
        Endless = true; // Makes this trial run repeatedly until ExitTrial() is called

        someCanvas.enabled = true;
    }

    protected override void OnNextRepetition() // Optional override
    {
        someText.text = "Repetition: " + CurrentRepetition;
    }

    protected override void OnTrialComplete() // Optional override
    {
        Debug.Log("Trial Complete. Yay.");
    }
}
```

The inspector would show this:
<img src="Images~/mytrial_example.png" width=400px alt="Example of a Trial" />

- The three methods: `OnTrialBegin()`, `OnNextRepetition()` and `OnTrialComplete()` are all optional. Your class can be entirely blank.
- Trials that have their property `Endless` set to `true` will run repeatedly until the `ExitTrial()` method is called. Phases can access this using `trial.ExitTrial()`, as `trial` refers to its parent Trial.
- Phases can also use the `trial.SkipToTrial()` method to immediately skip to another trial in its parent Block's hierarchy. This can be useful for creating simple menus.

### Block

A Block is a container of Trials. Each trial is run through one-by-one according to the order in the hierarchy. You will most likely only use the provided `BlankBlock` or `GenericBlock` (that has Unity Events), since most of the useful stuff is accessed through the Inspector.

```C#
public class BlankBlock : Block
{
    protected override void OnBlockStart() {} // Optional

    protected override void OnBlockEnd() {} // Optional
}
```

<img src="Images~/blank_block.png" width=400px alt="Blank Block" />

- **Trials To Shuffle**
  
  TODO: Explain.
  
  TODO: [Image explaining]

- **Utilities**
  - The Block Overview has a useful interface to quickly set its Trials repetitions and each Trial's Phases' duration and OnlyOnFirstRepetition properties.

## Structures Overview

This should help explain some concepts presented above:

![Giant overview diagram](Images~/main_diagram.svg)

## Event Manager

Stub: Event Manager only does one thing: `RaiseNextPhase()`.

## Data Logger

Stub: Data logger is optional, but very useful for collecting basic data about each repetition or trial.

## Install

Experiment Structures doesn't have any dependencies. You have two options: 
- Clone this repo into your `Packages/` directory in your Unity project. This is useful if you want to modify this package to your own liking.
- In the Package Manager, click the **+** sign at the top-left corner and hit [Add package from git URL...], then enter `https://github.com/prnthp/experiment-structures.git`

Once you've added Experiment Structures, go to **Assets**→**Create Experiment Structures Templates** to add some script templates to your `Asset/ScriptTemplates` directory. After restarting the Editor, you can just right-click in your Project tab and hit **Create**→**Experiment Structures**→**Phase**,**Trial** & **Block** just like you would with a new C# script! All the boilerplate is already added for you!

<!-- TODO: Publish to UPM -->

## Samples

### Basic

Stub: Bogus 2AFC stuff

### AEPsych-driven Samples

Hi Meta folks!

<!-- TODO: cool gif -->

See [/Samples~/AEPsychDriven/](Samples~/AEPsychDriven/)

### VR

Stub: Button user study from Chasm

## License

MIT License

Copyright © 2021 Jom Preechayasomboon
