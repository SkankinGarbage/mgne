local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()
local hero3 = getHero(2).getName()

speak(hero2, "Strange... the door ahead is unsealed!")
speak(hero3, "Everyone left without the real EXCALIBUR?")
speak("RADIO", "This is Jonas! Listen, " .. hero1 .. " I owe you an apology, but we can talk later. I need your help!")
speak(hero1, "Jonas?")
speak("RADIO", "The prince is here! Twentieth floor! Hurry!")
speak("RADIO", "... *click*")
speak(hero2, "Let's go!")

sceneSwitch('f15', true)
