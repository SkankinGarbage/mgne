local hero1 = getHero(0).getName()

speak("Jonas", "Just this guy to deal with and we're out!")
speak("Sekhmet", "I'm afraid that won't be so easy. I am Sekhmet. Who are you that challenges the masters?")
speak(hero1, hero1 .. ".")
speak("Sekhmet", "Well I'm afraid your luck runs out here, " .. hero1 .. ". If the missile won't kill you, then I will!")

sceneSwitch('sekhmet_dead', true)
battle('party_bossSekhmet', false, 'battle_boss')
