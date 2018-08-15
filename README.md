Player
======
_Framework for creating awesome slotcar AIs!_

Getting started
---------------
_TODO: Get some stuff here_

VSTS Build
----------
The VSTS build is located at [VSTS](https://teodoran.visualstudio.com/slotcar-ai/_build/index?context=mine&path=%5C&definitionId=1&_a=completed)

Test: How to get started as a workshop participant
--------------------------------------------------
1. Install Git, .NET Core and a suitable editor
2. Fork https://github.com/slotcar-ai/player
3. Clone your fork
4. Restore and build the Player project
```
$> cd player/Player
$> dotnet restore
$> dotnet build
```
5. Give your player a name and edit it some.
6. Test the player locally using `dotnet test` in the Player.Test folder.
7. If the tests worked, commit all your changes and push them with.
```
$> git add --all
$> git commit -m "My first lotacar AI!"
$> git push origin master
```
8. Navigate to your repo on GitHub and create a pull-request