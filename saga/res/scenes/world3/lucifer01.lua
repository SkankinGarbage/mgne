local hero1 = getHero(0).getName()
local hero2 = getHero(1).getName()
local hero3 = getHero(2).getName()
local hero4 = getHero(3).getName()

walk('hero', 2, 'NORTH')

speak("????", "Welcome, travelers!")
speak(hero4, "What is the place?")
speak("Prince", "I am the prince of the City of Angels. You may call me 'your majesty.'")
speak(hero3, "Your majesty?")
speak("Prince", "Very good.")
speak("Prince", "You come from Babel, right?")
speak(hero4, "In a way, yes.")
speak("Prince", "We have been building a machine to open the gate to Babel. You are the results of the first test! Welcome again!")
speak(hero1, "Everyone seems to want to open a way to Babel... Does that make you a 'master?'")
speak("Prince", "Some may call me that! But I am a kind and noble master who will bring my city peace and riches.")
speak(hero1, "Hm.")
speak("Prince", "Very well, our test is done until we can find more power.")
speak("Prince", "Travelers, you are welcome to take the train to the City of Angels and join me at my palace for a grand reception!")
speak("Prince", "And maybe next time the machine fires the gate will open up for good...")

path('lucifer', 'target_center', false)
wait(0.7)
path('guard1', 'target_left')
path('guard2', 'target_left')
path('guard3', 'target_right')
path('guard4', 'target_right')
wait(3.0)

speak(hero2, "Very suspicious...")
speak(hero1, "I wonder if the Brotherhood is in this world?")

sceneSwitch('lucifer01', true)
