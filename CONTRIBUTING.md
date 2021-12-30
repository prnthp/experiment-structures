# Contributing

We use fairly "standard" github contribution workflow:

1. Make an [Issue](https://github.com/prnthp/experiment-structures/issues/new)
2. Fork this repository (For further details, see https://docs.github.com/en/github/getting-started-with-github/fork-a-repo)
3. Develop changes to a new branch to your forked repository
4. Create a Pull Request from your forked repository against this repository
   1. Insert a reference in the description to the issue created earlier eg. "Closes #1" where "1" is the issue number
   2. Pull request description should answer these questions: "What has been changed" and "What is this for"

## Developing

One way to develop this Unity package is to create a new Unity Project and copy this package to its Assets folder.

This way .meta files (required by Unity) are generated automatically. Assets available in the package can now be tested and developed inside the project.

After making changes you can test your package by eg. installing it via Git URL:

Open `Packages/manifest.json` with your favorite text editor. Add following line to the dependencies block:
```json
    {
        "dependencies": {
            "com.prnthp.experiment-structures": "https://github.com/YOUR_USER/experiment-structures.git"
        }
    }
```

For further details, see [Unity docs about custom packages](https://docs.unity3d.com/Manual/CustomPackages.html).