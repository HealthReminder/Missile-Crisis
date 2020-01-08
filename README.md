# Missile-Crisis

A multiplayer war game with up to 10 player per match.
You play as a leader controlling its own nation in a Cold War scenario.

Every player gets to place a few missile silos. But players only knows where its own are. 
The winner is the one left with silos still up.
You can shoot atomic missiles from your silos but they leave trails behind so your enemies know where it came from.

This game was created in Unity3D using the Photon plugin to handle multiplayer and room connections. 
I had special care on dealing with players quitting the match or with hosts delaying the gameplay in a way that the game is never frozen or slowed down for the other players.

It features room selection and creation. The maps and nation distribution are procedurally generated using an amalgamae of perlin noise and celullar-automata-like algorithms. This game is cross-platform but is currently only available on PC.
