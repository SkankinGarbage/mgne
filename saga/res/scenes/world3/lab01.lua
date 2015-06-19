local hero1 = getHero(0).getName()
local hero3 = getHero(2).getName()

speak("Guard", "Sorry, no access past -")
speak("Zkauba", "We are the Brotherhood! Prepare to die!")

sceneSwitch('lab01', true)
battle('encounter_lvl06_HATAMOTO1-2_AUTOMATON1-2', false)

speak("Zkauba", "Come, let's find the scientists in charge here!")
