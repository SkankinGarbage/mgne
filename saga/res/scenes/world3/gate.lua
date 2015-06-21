local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()
local hero3 = getHero(2).getName()
local hero4 = getHero(3).getName()

wait(.8)
walk('hero', 3, 'NORTH', false)
walk('zkauba', 3, 'NORTH', false)
walk('aven', 3, 'NORTH', true)

playSound('explode')
shake(1.0)

speak(hero2, "What's going on?")
speak("Aven", "The gate is opening!")

sceneSwitch('visitors_appear', true)
walk('david', 1, 'SOUTH', false)
walk('lara', 1, 'SOUTH', true)
walk('lara', 1, 'EAST')
face('lara', 'SOUTH')

speak(hero3, "David?")
speak("Aven", "And Lara. Where have you been?")
speak("David", hero1 .. "! Our home world is under attack!")
speak("Lara", "The prince found a path to another world. Jonas is there now trying to hold him off.")
speak("David", "It's chaos! The city of Hero is in ruins!")
speak(hero4, "It can't be!")
speak("Aven", "Terrible...")
speak(hero1, "What can we do?")
speak("Aven", "The Brotherhood must reseal the gate.")
speak(hero1, "But what about our home?")
speak("Aven", "Well... I can point the gate at a shortcut to your world.")
speak("David", hero1 .. ", you guys have got to come and help!")
speak("Aven", "Then stand back.")

walk('lara', 1, 'SOUTH', false)
walk('david', 1, 'SOUTH', true)
face('lara', 'NORTH')

playSound('flare')
shake(.7)
wait(1.0)

face('lara', 'NORTH')
face('david', 'NORTH')

speak("Aven", "There. You should be able to get back to your home through Babel.")
speak("Lara", "Aven and I will stay here and find a way to repair this gate.")
speak("David", "I really don't know who either of you are, so " .. hero1 .. " I'm coming with you!")

pathEvent('david', 'hero')
addMember('chara_david2')

fade('white')
wait(1.0)
sceneSwitch('world3_gate', true)
fade('normal')
wait(1.0)
