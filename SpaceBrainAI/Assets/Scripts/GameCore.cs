using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameCore : MonoBehaviour
{
    #region Singleton Setup
    private static GameCore instance;
    public static GameCore Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameCore();
            }
            return instance;
        }
    }

    //awake is called before first frame. 
    void Awake()
    {
        instance = this;
    }
    #endregion

    public static GameState player;
    public Dictionary<string, GameEvent> ALL_EVENTS;//all game events database
    public Dictionary<string, GameEvent> PlayerDeck;//player deck pulls events from ALL_EVENTS
    public GameEvent CurrentEvent;//currently active card or event
    public Choice genericContinue;//adds an empty choice to continue if an event has none

    public void initialize_game()
    {
        print("Welcome to SPACE BRAIN! THE GAME!" + "\n");
        print(
        "You are the Space Brain. You are an AI tasked with maintaining " + "\n" +
        "a fledgling space colony by increasing its overall efficiency. " + "\n" +
        "This means LOW COST and HIGH OUTPUT (and HIGH MORALE is nice but " + "\n" +
        "that's just a bonus!) You affect these stats by responding to " + "\n" +
        "EVENTS that are brought to your attention by your various " + "\n" +
        "autoadvisors."
        );

        player = new GameState(10.0f,2.0f,5.0f,15);
        print_to_screen();
        load_game_events();
        initialize_deck(Level.ONE);
        draw_next_event();

    }

    void draw_next_event()
    {
        if (PlayerDeck.Count > 0)
        {
            var CurrentEventPair = PlayerDeck.ElementAt(RandomNumber(0, PlayerDeck.Count));//grab random element
            CurrentEvent = CurrentEventPair.Value;
            print_event(CurrentEvent);
            PlayerDeck.Remove(CurrentEventPair.Key);
        }
        else
        {
            Debug.LogError("We've run out of cards.");
        }
    }

    void print_event(GameEvent e)
    {
        print(e.description);
        print("");
        print("NOTE: choices are sent with the submit command.");
        print("- - - - - - - - - - - - - - - - - - - - - - - - -");

        //Debug.Log(e.choices);
        if (e.choices?.Any() != true)//if event has no choices, give it a generic continue
        { e.choices = new List<Choice>() { genericContinue }; }

        int i = 1;
        foreach (Choice c in e.choices)
        {
            print(i + ": " + c.description);
            i++;
        }
    }

    public void submitChoice(int input)
    {
        if(input <= CurrentEvent.choices.Count && input > 0)
        {
            Console.Instance.ClearLog();//clear out console log(for aesthetics)
            var PlayerChoice = CurrentEvent.choices[input - 1];
            player = PlayerChoice.update_game(player);//update stats

            if (PlayerChoice.direct_events != null)//if our choice has direct events associated
            {
                var nextDirectEvent = get_next_event(PlayerChoice.direct_events);//pull one from direct event pile
                
                if (!PlayerDeck.ContainsKey(nextDirectEvent.Key))//check if key pair already exists in dictionary
                {
                    PlayerDeck.Add(nextDirectEvent.Key, nextDirectEvent.Value);
                }
            }
            if (PlayerChoice.later_events != null)//if our choice has later events associated
            {
                foreach(GameEventProbability e in PlayerChoice.later_events)
                {
                    if(UnityEngine.Random.value > e.probability)
                    {
                        if (!PlayerDeck.ContainsKey(e.eventKeyPair.Key))//check if key pair already exists in dictionary
                        {
                            PlayerDeck.Add(e.eventKeyPair.Key, e.eventKeyPair.Value);
                        }
                    }
                }
            }
            print_to_screen();
            draw_next_event();
        }
        else
        {
            Debug.LogWarning("Bad number. bitch.");
        }
    }

    void initialize_deck(Level startingLevel)
    {
        //randomize
        PlayerDeck = new Dictionary<string, GameEvent>(); //ALL_EVENTS.shuffle(); //player deck should ideally start with LEVEL ONE cards
        foreach (string key in ALL_EVENTS.Keys)
        {
            if (ALL_EVENTS[key].level == startingLevel)
                PlayerDeck.Add(key, ALL_EVENTS[key]);
        }
        PlayerDeck = PlayerDeck.shuffle(); //shuffle order of events
    }

    public void load_game_events()
    {
        
        ALL_EVENTS = new Dictionary<string, GameEvent>();
        ALL_EVENTS.Add("FECAL_INTERROGATION_PERP", new GameEvent(Level.NONE, "The totally guilty perp behind the fecal pipe burst has been found. Congratz.", null));
        ALL_EVENTS.Add("FECAL_ZOMBIES", new GameEvent(Level.NONE, "Quarantined shitpools from the overflow have leaked into class-D residences, resulting in zombies. Fuck you.", null));
        ALL_EVENTS.Add("FECAL_PROCESSOR", new GameEvent(
            Level.ONE,
            "The Fecal Processor was sabotaged last night. " + "\n" + "The street tubes are full of feces.",
            new List<Choice>()
            {
                new Choice(
                    "Interrogate every man woman and child until the perpetrator is found.",
                    g => new GameState(g.cost + 10.0f, g.output, g.morale / 2, g.population),
                    new List<GameEventProbability>{new GameEventProbability(ALL_EVENTS.GetEntry("FECAL_INTERROGATION_PERP"), 1.0f)},
                    null
                ),
                new Choice(
                    "Quarantine the affected tubes.",
                    g => new GameState(g.cost / 1.5f, g.output / 1.2f, g.morale / 2, g.population),
                    null,
                    new List<GameEventProbability>{ new GameEventProbability(ALL_EVENTS.GetEntry("FECAL_ZOMBIES"), 0.5f) }
                )
            }
        ));

        ALL_EVENTS.Add("HORROR_TOILET", new GameEvent(
            Level.ONE,
            "There are strange noises coming from the sector 3 personal sanitation bank.",
            new List<Choice>()
            {
                new Choice(
                    "Send team of investigators.",
                    g => new GameState(g.cost + 10.0f, g.output, g.morale / 2, g.population),
                    null,
                    null
                ),
                new Choice(
                    "Close off station.",
                    g => new GameState(g.cost / 1.5f, g.output / 1.2f, g.morale / 2, g.population),
                    null,
                    null
                )
            }
        ));

        ALL_EVENTS.Add("TABLE_SHAKE", new GameEvent(
            Level.ONE,
            "Messhall 48a is experiencing a shortage of tables.",
            new List<Choice>()
            {
                new Choice(
                    "Allocate industrial Fabrimakers to table production.",
                    g => new GameState(g.cost + 10.0f, g.output, g.morale / 2, g.population),
                    null,
                    null
                ),
                new Choice(
                    "Allow take-out production.",
                    g => new GameState(g.cost / 1.5f, g.output / 1.2f, g.morale / 2, g.population),
                    null,
                    null
                )
            }
        ));
        genericContinue = new Choice("Submit 1 to Continue...", g => new GameState(g.cost, g.output, g.morale, g.population), null, null);
    }

    //given a set of probabilities, choose one
    //from http://www.vcskicks.com/random-element.php
    KeyValuePair<String, GameEvent> get_next_event(List<GameEventProbability> events)
    {
        double r = UnityEngine.Random.value;
        double cumulative = 0.0;
        for (int i = 0; i < events.Count; i++)
        {
            cumulative += events[i].probability;
            if (r < cumulative)
            {
                return events[i].eventKeyPair;
            }
        }
        Debug.LogWarning("Probable Selection of Next Event Failed. Defaulting.");
        return events[0].eventKeyPair;
    }

    public void print_to_screen()
    {
        print("");
        print("Way to go, champ! Your colony's stats are:");
        print("COST = " + player.cost.ToString("0.00"));
        print("OUTPUT = " + player.output.ToString("0.00"));
        print("MORALE = " + player.morale.ToString("0.00"));
        print("POPULATION = " + player.population);
        print("EFFICIENCY = " + player.efficiency.ToString("0.00"));
        print("");
    }
    
    //Function to get a random number from 
    private static readonly System.Random random = new System.Random();
    private static readonly object syncLock = new object();
    public static int RandomNumber(int min, int max)
    {
        lock (syncLock)
        { // synchronize
            return random.Next(min, max);
        }
    }
}

public enum Level
{
    NONE = 0,//represents followup cards that don't organically go in the deck
    ONE = 1,
    TWO = 2,
    THREE = 3,
    FOUR = 4,
    FIVE = 5,
}

public class Choice
{ 
    public string description;
    public Func<GameState, GameState> update_game;
    public List<GameEventProbability> direct_events;
    public List<GameEventProbability> later_events;

    public Choice(string inputDescription, Func<GameState, GameState> inputUpdate_game, List<GameEventProbability> inputDirect_events, List<GameEventProbability> inputLater_events)
    {
        this.description = inputDescription;
        this.update_game = inputUpdate_game;
        this.direct_events = inputDirect_events;
        this.later_events = inputLater_events;
    }

    bool validate()
    {
        float sum = 0;
        foreach (GameEventProbability e in direct_events)
        {
            sum += e.probability;
        }
        return sum <= 1;
    }
}

public class GameEvent
{
    public Level level;
    public string description;
    public List<Choice> choices;

    public GameEvent(Level inputLevel, string inputDescription, List<Choice> inputChoices)
    {
        this.level = inputLevel;
        this.description = inputDescription;
        this.choices = inputChoices;
    }
}

public class GameEventProbability
{
    public KeyValuePair<String, GameEvent> eventKeyPair;
    public float probability;

    public GameEventProbability(KeyValuePair<String, GameEvent> input, float v)
    {
        this.eventKeyPair = input;
        this.probability = v;
    }
}

public struct GameState
{
    public float cost;
    public float output;
    public float morale;
    public int population;

    public GameState(float v1, float output, float v2, int population) : this()
    {
        this.cost = v1;
        this.output = output;
        this.morale = v2;
        this.population = population;
    }

    public float efficiency
    {
        get
        { return cost / output;}
    }
}

public static class DictionaryExtensions
{   
    //from https://jigneshon.blogspot.com/2013/08/c-snippet-shuffling-dictionary-beginner.html
    public static Dictionary<TKey, TValue> shuffle<TKey, TValue>(
       this Dictionary<TKey, TValue> source)
    {
        System.Random r = new System.Random();
        return source.OrderBy(x => r.Next())
           .ToDictionary(item => item.Key, item => item.Value);
    }
    //https://stackoverflow.com/questions/1619090/getting-a-keyvaluepair-directly-from-a-dictionary
    public static KeyValuePair<TKey, TValue> GetEntry<TKey, TValue>
    (this IDictionary<TKey, TValue> dictionary,
     TKey key)
    {
        return new KeyValuePair<TKey, TValue>(key, dictionary[key]);
    }
}