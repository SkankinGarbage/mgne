local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()
local hero4 = getHero(3).getName()

face('hero', 'SOUTH')

wait(1.5)
speak("David", hero1 .. ", before we go anywhere, you have to be careful!")
speak(hero1, "Of course.")
speak("David", "No, I mean... The master here, we can't -")
speak("David", "Even with my axe, I can't hurt him! It's not natural.")
speak(hero4, "Hm, the masters we've fought so far couldn't stand against our weapons.")
walk('hero', 1, 'SOUTH')
wait(0.8)
speak("David", "Ahh! He's here! " .. hero1 .. ", run!")

battle('encounter_bossAresInvincible', true, 'ffl1_boss')

speak(hero2, "It must be his shield.")
speak("David", "He's been destroying everything in his path!")
speak(hero1, "Quick, let's take cover.")

sceneSwitch('ares01', true)
