local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()
local hero3 = getHero(2).getName()
local hero4 = getHero(3).getName()

playBGM('ffl3_steslos')

speak(hero1, "Jonas! Janine!")
speak("Janine", "Stay back, " .. hero1 .. ".")
pathEvent('hero', 'herosquare')
wait(1.0)

speak("Prince", "You're a fool, Jonas. That piece of EXCALIBER is fake. I couldn't care less if you destroy it.")
speak("Jonas", "What? No... But you're too late.")
speak("Jonas", hero1 .. " showed up and soon Aven and the brotherhood will be here to finish you off. And you still can't break Babel's seal.")
speak("Prince", "Oh? I don't need EXCALIBER any more. I have MASAMUNE.")
speak(hero1, "Janine! You wouldn't give it to him!")
speak("Janine", "No need. I'll break the seal myself.")

walk('janine', 3, 'EAST')
face('janine', 'NORTH')
wait(1.0)
playSound('roar')
wait(0.4)
face('janine', 'SOUTH')

speak("Jonas", "Janine! You monster! What have you done?")
speak("Janine", "I'm sorry you'll never understand, Jonas.")
speak("Prince", "Very helpful, Janine, but I'm afraid you're no longer necessary.")
speak("Jonas", "No!")

walk('prince', 2, 'EAST', false)
walk('jonas', 2, 'WEST', false)
fade('white')
wait(1.0)
face('prince', 'NORTH')
sceneSwitch('jonas_dead', true)
fade('normal')
wait(1.0)

speak("Janine", "Jonas... You didn't have to...")
speak("Prince", "Pah. Who cares.")
face('prince', 'SOUTH')
speak("Prince", "The seal is broken and I'll become a true god. But first, for the rest of you.")
