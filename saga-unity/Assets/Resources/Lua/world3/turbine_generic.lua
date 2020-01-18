local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()

playSound('explode')
shake(.5)

speak(hero1, "Another turbine down.")
speak(hero2, "It looks like there are four of them.")
