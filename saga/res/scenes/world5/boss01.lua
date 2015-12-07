local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()
local hero3 = getHero(2).getName()
local hero4 = getHero(3).getName()

playBGM('ffl1_paradise')

speak("Aven", "Oh, " .. hero1 .. " and friends, you finally caught up.")
speak("Janine", "Watch yourself, Aven. Jonas died for you. His blood is on your hands.")
speak(hero1, "You have a lot of explaining to do.")
speak("Aven", "It's actually quite simple. Ever since I stood before that sealed door, I knew I needed to find my way back to Babel's heartland.")
speak("Aven", "Now I can finally become the master I've always known I was.")
speak(hero2, "We don't need more masters.")
speak(hero3, "If you're joining them, we'll have to fight you.")
speak("Aven", "Why? Those old demons are dead or sealed, thanks to me. With the power of a master, I can bring peace to the rest of Babel's worlds.")
speak(hero4, "That doesn't sound so bad...")
speak("Aven", "I'll rule them as the rightful king, of course.")
speak("Aven", hero1 .. ", you should join me. Just drink from the well, and you can join the master of Babel!")
speak(hero1, "I don't think so, Aven.")
face('aven', 'NORTH')
speak("Aven", "Very well...")
wait(1.0)
face('aven', 'SOUTH')
speak("Aven", "But don't doubt the greatness of my power!")

sceneSwitch('aven_dead', true)
battle('party_bossFinal', false, 'ffl3_xagor')

playBGM(nil)
wait(0.7)

sceneSwitch('brotherhood_final', true)
walk('lara', 9, 'NORTH', false)
walk('zkauba', 9, 'NORTH', false)

speak("Zkauba", hero1 .. "!")
speak("Lara", "Aven is defeated?")
speak("Janine", "He's dead.")
speak(hero1, "The last of the masters is gone.")
speak(hero2, "And now we can go home.")
speak(hero1, "To our world.")

wait(1.0)
play('world5/outro.lua')