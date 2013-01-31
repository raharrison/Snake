# Snake

An implementation of the classic game Snake in C#.

The aim of the game is to get the highest score possible by directing the head of the snake to the yellow food placed randomly around the playing area.
As the food is eaten, the snake grows by five blocks which makes it harder to control.

The game is over when the head of the snake hits any of the four walls or any part of it's body.

Controls - 

* Move Head Left = Left Arrow
* Move Head Right = Right Arrow
* Move Head Up = Up Arrow
* Move Head Down = Down Arrow
* New Game = Space
* Pause = P
* Increase Game Speed = NumPad +
* Decrease Game Speed = NumPad -

This implementation includes a queue based turning system meaning that input is not lost when the player turns direction twice during one game tick.

Possible improvements - 

* Ability to re-scale the game area
* Introduce obstacles
* Option to 'go through' walls, arriving on the opposite side of the game area