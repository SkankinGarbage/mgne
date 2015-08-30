local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()
local hero3 = getHero(3).getName()

speak("Demon", "You can't run! Now show us the way to Elan!")
speak("????", "Aaah!")

walk('demon01', 1, 'SOUTH')
walk('demon03', 1, 'WEST')
walk('demon02', 1, 'SOUTH')
face('demon02', 'WEST')

speak(hero2, "What should we do?")
speak(hero1, "Help of course!")

sceneSwitch('itamor01', true)
battle('encounter_lvl10_DEMON LORD1-2_RAT KING0-1', false)
wait(0.5)

speak(hero3, "Are you alright?")
speak("Itamor", "Yes. My name is Itamor. I'm a scout from Elan but Moloch's men caught me here.")
speak(hero1, "I'm " .. hero1 .. ". We'll help you escape.")
speak("Itamor", "Thank you. I can't fight, but as thanks I can guide you to Elan.")

pathEvent('itamor', 'hero')
sceneSwitch('itamor_freed', true)

speak("Itamor", "First let's get out of Sarnath!")
