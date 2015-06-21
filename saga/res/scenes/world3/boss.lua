local hero1 = getHero(0).getName()
local hero2 = getHero(2).getName()

playBGM('ffl1_boss')
speak("Zkauba", hero1 .. "! Get back!")
speak(hero1, "What?")
speak("Zkauba", "The prince went through the gate days ago! It's a trap!")
speak("????", "INTRUDER ALERT. GET THEM! GET THEM!")

sceneSwitch('machine_dead', true)
battle('party_bossMachine', false, 'ffl1_boss')
face('hero', 'NORTH')
playBGM('ffl3_ruins')

speak("Zkauba", "You saved me. I underestimated you, " .. hero1 .. ".")
speak(hero1, "It was nothing.")
speak("Zkauba", "The prince is already in Babel, and this whole story about energy was a trick!")
speak("Zkauba", "With a head start like this, who knows what he could do.")
speak(hero2, "Maybe Aven will have an answer.")

fade('white')
wait(1.0)
teleport('world3/base.tmx', 13, 18, 'EAST', false)
fade('normal')
wait(1.0)

sceneSwitch('left_power_plant', true)
