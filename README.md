# How to use

Import this folder as a new Unity project and open it. Now go into the play mode and press your Jump button(Space) in the game window to randomly spawn the example creature. To create a new spawnzone create a new GameObject and assign CreatureSpawnzones to it. Each CreatureSpawnzones can have multiple shapes aka. spawnzones and all of them will be registered to the Singleton SpawnManager. Make sure to assign the Creature field in CreatureSpawnzones so that the SpawnManager knows which spawnzone is for which creature. You can have multiple CreatureSpawnzones with the same creature or just create multiple shapes in one CreatureSpawnzones, the effect is the same.

# Version

Made with Unity 2018.4.0f1 LTS
