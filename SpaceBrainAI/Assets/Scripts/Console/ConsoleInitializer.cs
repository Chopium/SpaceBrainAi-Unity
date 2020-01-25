using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;

public class ConsoleInitializer : MonoBehaviour 
{
    void Start()
	{
        var repo = ConsoleCommandsRepository.Instance;
        #region Basic Commands
		    repo.RegisterCommand("map", Map);
            repo.RegisterCommand("reload", ReloadLevel);
            repo.RegisterCommand("clear", Clear);
            repo.RegisterCommand("exit", Exit);
            //repo.RegisterCommand("reset", ResetPosition);
            //repo.RegisterCommand("quality", SetQuality);
        #endregion
        #region Graphics commands
        //    repo.RegisterCommand("cl_drawtrees", cl_drawtrees);
        //    repo.RegisterCommand("cl_lod", cl_lod);
        //    repo.RegisterCommand("cl_farclipplane", cl_camerafarplane);
        //    repo.RegisterCommand("cl_drawshadows", cl_drawshadows);
        #endregion

        repo.RegisterCommand("start", startGame);
        repo.RegisterCommand("submit", submitChoice);
    }
    #region Older Commands
  
    public string cl_lod(params string[] args)
    {
        float oldLOD = QualitySettings.lodBias;
        float newLOD = 0f;
        float.TryParse(args[0], out newLOD);
        if (newLOD != oldLOD && newLOD > 0f && newLOD < 10f)
        { QualitySettings.lodBias = newLOD; }
        return "Lod setting is now at "+ QualitySettings.lodBias;
    }
    public string cl_camerafarplane(params string[] args)
    {
        float oldLOD = Camera.main.farClipPlane;
        float newLOD = 1000f;
        float.TryParse(args[0], out newLOD);
        if (newLOD != oldLOD && newLOD > 0f && newLOD < 100000f)
        { Camera.main.farClipPlane = newLOD;
            //atmospheric effects should be adjusted here, or we could take care of it through rendering changes
        }
        return "Camera Far Clip Plane at :  " + Camera.main.farClipPlane;
    }
    public string cl_drawshadows(params string[] args)
    {
        bool enableshadows = false;
        bool.TryParse(args[0], out enableshadows);
        QualitySettings.shadows = enableshadows ? ShadowQuality.All : ShadowQuality.Disable;

        return "shadows :  " + enableshadows;
    }
    public string DrawGUI(params string[] args)
    {
        bool enableGUI = true;
        bool.TryParse(args[0], out enableGUI); //(Light[]) GameObject.FindObjectsOfType (typeof(Light));
        //QualitySettings.shadows = enableGUI ? ShadowQuality.All : ShadowQuality.Disable;
        Canvas[] targets = (Canvas[])Resources.FindObjectsOfTypeAll(typeof(Canvas));
        foreach (Canvas t in targets)
        {
            t.gameObject.SetActive(enableGUI);
        }

        return "GUI Draw :  " + enableGUI;
    }
    public string cl_drawtrees(params string[] args) //0 to false, 1 to true
    {
        bool enabletrees = false;
        bool.TryParse(args[0], out enabletrees);
        GameObject[] gameObjectArray = GameObject.FindGameObjectsWithTag("Trees");

        foreach (GameObject go in gameObjectArray)
        {
            foreach (Transform child in go.transform)
            {
                child.gameObject.SetActive(enabletrees);
            }
        }
        return "Trees set to : " + enabletrees;
    }
  
    public string Clear(params string[] args)
    {
        if (args.Length > 0)
            if (args.Length > 0)
            {
                if (args[0].Contains("scene"))
                {
                    //ClowderBridge.Instance.ClearScene();
                }
                if (args[0].Contains("resources"))
                {
                    Resources.UnloadUnusedAssets();
                }
                if (args[0].Contains("log"))
                {
                    Console.Instance.ClearLog();
                }
            }
            else
            {
                //ClowderBridge.Instance.ClearScene();
            }
                return ("");
    }
    public string SetQuality(params string[] args)
    {

        string output = "";
        int input = QualitySettings.GetQualityLevel();
        if (args.Length > 0)
        {
            if (args[0].Equals("list"))
            {
                for (int i = 0; i < QualitySettings.names.Length; i++)
                {
                    output += "\n" + QualitySettings.names[i] + " [" + i + "]";
                }
            }
            else
            {
                int.TryParse(args[0], out input);
                if (input != QualitySettings.GetQualityLevel())
                {
                    QualitySettings.SetQualityLevel(input);
                }
            }
            //Debug.Log(QualitySettings.names[input]);
        }
        output+= "\nCurrent Quality Setting: " + QualitySettings.names[input] + " [" + input+"]";
        return (output);
    }
    public string Map(params string[] args)
		{
		//check if map name is valid
		var mapname = args[0];
        int mapNumber;
        if (int.TryParse(mapname, out mapNumber))
        {
            SceneManager.LoadSceneAsync(mapNumber);
            return ("Attempting to Load Map");
            /*
            if (mapNumber > 0 )
            {
                SceneManager.LoadSceneAsync(mapNumber);
                return ("Attempting to Load Map");
            }
            else
            {
                return ("Cannot go to map zero");
            }*/
        }
        //Debug.Log(SceneManager.GetSceneByName(mapname).buildIndex);
        if(SceneManager.GetSceneByName(mapname).buildIndex < 1)
        {
            return ("Cannot got to map zero");
        }
        else
        SceneManager.LoadSceneAsync(mapname);
        return ("Attempting to Load Map...");
	}
    public string ReturnObject(params string[] args)
    {
        //Debug.Log("Returning Object");
        //PlayerInterface.instance.ReturnInteractiveObject();
        return ("Object Returned!");
    }
    public string ResetPosition(params string[] args)
    {/*
        //PlayerInterface.instance.ResetPlayer();
        var target = GameObject.FindObjectsOfType<R3D_SpawnPoint_Test01>() as R3D_SpawnPoint_Test01[];
        foreach(R3D_SpawnPoint_Test01 instance in target)
        {
            if(instance.MainSpawnpoint)
            {
                instance.SpawnHere();
            }
        }*/
        return ("Player Reset!");
    }
    public string ReloadLevel(params string[] args)
    {
        //check if map name is valid
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        //return ("Attempting to Reload Map...");
        /*
        foreach (GameObject o in Object.FindObjectsOfType<GameObject>())
        {
            Destroy(o);
        }*/
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        return ("Attempting to Reload Map...");
    }

    public string Exit(params string[] args)
		{
        StartCoroutine("Quit");
        return "Exiting";
		}
    public IEnumerator Quit()
    {
        #if UNITY_EDITOR
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        #endif

        //PlayerPrefs.DeleteAll();
        //yield return new WaitForEndOfFrame();
        //System.Diagnostics.Process.GetCurrentProcess().Kill();

        yield return new WaitForEndOfFrame();
        Application.Quit();
        yield break;

    }

    #endregion
    public string startGame(params string[] args)
    {
        //could hand it starting parameters from argument
        GameCore.Instance.initialize_game();
        return "";
    }
    public string submitChoice(params string[] args)
    {
        //check if map name is valid
        var choiceInput = args[0];
        int choice;
        if (int.TryParse(choiceInput, out choice))
        {
            GameCore.Instance.submitChoice(choice);
            return ("");
        }
        else Debug.LogWarning("Non Parsable Number Entered");
        return ("");
    }
}