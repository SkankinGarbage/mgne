local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()
local hero3 = getHero(2).getName()
local hero4 = getHero(3).getName()

walk('hero', 2, 'EAST')
walk('janine', 2, 'EAST')
walk('janine', 2, 'SOUTH')
walk('janine', 2, 'WEST')

speak("Janine", "Could that be... " .. hero1 .. "?")
speak(hero1, "Janine! What are you doing here?")
speak("Zkauba", "You know this woman? She's a minion of the prince! She designed the gate machine!")
speak(hero1, "You've sided with the masters?")
speak("Janine", "Never! It's a long story, but...")
speak("Janine", "The only way to defeat the masters is to use the power within Babel. So we must open the gates! It's the only way!")
speak("Zkauba", "Mortals can never hope to defeat the masters. Aven says sealing them is the only way.")
speak("Janine", "We can't give up, either! I won't let the prince eat this world.")
speak("Zkauba", "Then why do you help him?")
speak("????", "They went this way!")
speak("Zkauba", "Guards! Quick, let's kidnap this scientist and go.")
speak("Janine", hero1 .. ", we'll meet again!")

walk('janine', 3, 'WEST')
face('hero', 'WEST')
walk('janine', 3, 'WEST')

wait(0.5)

walk('hero', 3, 'EAST')
face('hero', 'WEST')

sceneSwitch('lab_guards', true)
walk('guard1', 3, 'EAST', false)
walk('guard2', 3, 'EAST', false)
walk('guard3', 3, 'EAST', true)
walk('guard1', 1, 'SOUTH', false)
walk('guard2', 1, 'EAST', false)
walk('guard3', 1, 'EAST', true)
face('guard1', 'EAST')
walk('guard2', 1, 'NORTH', false)
walk('guard3', 1, 'EAST', true)
face('guard2', 'EAST')

speak("Guard", "The Brotherhood! Men, arrest these scums!")

fade('white')
wait(1.0)
sceneSwitch('janine01', true)
sceneSwitch('lab_guards', false)
teleport('world3/prison01.tmx', 21, 21, 'NORTH', false)
fade('normal')
wait(2.0)

speak("Zkauba", "Now look what you've done. Why didn't you stop that woman?")
