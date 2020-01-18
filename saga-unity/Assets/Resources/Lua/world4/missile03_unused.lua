local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()

walk('guard1', 1, 'EAST', false)
walk('guard2', 1, 'WEST')
face('guard1', 'SOUTH')
face('guard2', 'SOUTH')

playSound('explode')
shake(.5)

sceneSwitch('missile_bars_1', true)
wait(.15)
sceneSwitch('missile_bars_2', true)
wait(.15)

face('hero', 'NORTH')
speak("Jonas", "What??")

walk('guard1', 'NORTH', 6, false)
walk('guard2', 'NORTH', 6)

speak(hero2, "We're trapped!")
speak(hero1, "Jonas, what is this place?")

sceneSwitch('missile02', true)
