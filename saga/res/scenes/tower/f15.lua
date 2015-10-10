local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()
local hero3 = getHero(2).getName()

speak(hero1, "Strange... I thought we'd find Jonas or Aven here.")
speak(hero2, "Look, the door ahead is unsealed!")
speak(hero3, "Everyone left without the real EXCALIBER?")

sceneSwitch('f15', true)
