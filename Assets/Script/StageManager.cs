using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    #region Properties
    [Header("Menu")]
    [SerializeField] Transform stageDefault;
    [SerializeField] Transform contentStage;
    [SerializeField] Transform tutorial;
    [SerializeField] ScrollRect scrollStage;
    [SerializeField] Text stageUnlockedText;

    [Header("Line")]
    [SerializeField] GameObject line_h;
    [SerializeField] GameObject line_v;

    private List<Stage> allStage;
    private const int maxStage = 1000;                                            ///Requirement max stage is 999, but declaration is 1000
    private const int minStage = 1;
    private const int offsetChangeCorner = 4;                                     ///After 4 stage, change corner once
    private const int loopCount = 250;                                            ///loopCount = maxStage/offsetChangeCornet
    private const string keyStageIndex = "Index";
    private const string keyLockIcon = "Lock";
    private const string keyStarIcon = "Star";
    private const string keyUnlocked = "Stage Unlocked";
    private const string keyLineHorizontal = "Line_h(Clone)";
    private const string keyStageIcon = "Icon";
    private int stageUnlocked;
    private int amountStar;

    public static int StageSelected;

    #endregion

    void Start()
    {
        LoadStage();
    }

    private void LoadStage()
    {
        allStage = SaveData.LoadStage();

        if (allStage != null)
        {
            InitializeStage(allStage);
            int stageUnlocked = PlayerPrefs.GetInt(keyUnlocked);
            SetInfoByFormat(stageUnlocked);
        }
        else
        {
            allStage = new List<Stage>();
            InitializeStage();
        }
    }

    private void InitializeStage()
    {
        for (int i = minStage; i <= maxStage; i++)
        {   
            Stage stage = new Stage(i);
            allStage.Add(stage);
        }

        ZigZagFormat(allStage);
        SetUnlocked(allStage);
        SetRandomStar(allStage);
        InitializeStage(allStage);   
        SaveData.SaveStage(allStage);
    }

    private void InitializeStage(List<Stage> allStage)
    {
        foreach (Stage stage in allStage)
        {   
            Transform newStage = Instantiate(stageDefault, contentStage);
            SetUnlocked(newStage, stage.UnLocked);
            SetStarIcon(newStage, stage.AmountStar, stage.UnLocked);
            SetEvent(newStage,stage.Level, stage.UnLocked);
            SetLevelLabel(newStage, stage.Level);
            AddLine(newStage, stage.IsHoderLineH, stage.IsHoderLineV);
            if (stage.Level == maxStage)
            {
                HidenStageHaveMaxIndex(newStage);
            }
        }
        SetTutorialFirstStage();
    }

    private void SetUnlocked(List<Stage> allStage)
    {
        stageUnlocked = Random.Range(minStage, maxStage);
        SetInfoByFormat(stageUnlocked);
        PlayerPrefs.SetInt(keyUnlocked, stageUnlocked);

        foreach (Stage stage in allStage)
        {
            if (stage.Level < stageUnlocked)
            {
                stage.UnLocked = true;
            }
            else
            {
                stage.UnLocked = false;
            }
        }
    }

    private void SetRandomStar(List<Stage> allStage)
    {
        foreach (Stage stage in allStage)
        {
            if (stage.UnLocked)
            {
                amountStar = Random.Range(0, 3);
                stage.AmountStar = amountStar;
            }
        }
    }


    /// Set level stage and hoder value by ZigZag format
    private void ZigZagFormat(List<Stage> allStage)
    {
        int levelStage = 0;
        int indexContent = 0;
        int offsetZigZag = 1;
        int offsetLineV = -1;

        ///Stage Tutorial hoder first line horizontal
        allStage[0].IsHoderLineH = true;

        for (int i = 0; i < loopCount; i++)
        {   

            ///After change Corner, set value for hoder
            for (int j = 0; j < offsetChangeCorner; j++)
            {
                levelStage += offsetZigZag;
                offsetLineV += offsetZigZag;
                allStage[indexContent].Level = levelStage;
                indexContent++;
            }
            if(indexContent< maxStage)
            {
                allStage[indexContent].IsHoderLineH = true;
                allStage[offsetLineV].IsHoderLineV = true;
            }
            
            levelStage = levelStage + offsetChangeCorner + offsetZigZag;
            offsetLineV = offsetLineV + offsetChangeCorner + offsetZigZag;
            offsetZigZag = -offsetZigZag;
        }
    }

    
    private void HidenStageHaveMaxIndex(Transform stage)
    {
        ///Resize line
        var line_H = stage.Find(keyLineHorizontal);
        if (line_H != null)
        {
            line_H.GetComponent<RectTransform>().localPosition += new Vector3(120, 0);
        }

        ///Hiden Icon and event
        stage.Find(keyStageIndex).gameObject.SetActive(false);
        stage.Find(keyLockIcon).gameObject.SetActive(false);
        stage.Find(keyStarIcon).gameObject.SetActive(false);
        stage.Find(keyStageIcon).gameObject.SetActive(false);
        Destroy(stage.GetComponent<Image>());
        Destroy(stage.GetComponent<Button>());   
    }

    private void SetUnlocked(Transform stage, bool unLocked = false)
    {   
        stage.Find(keyLockIcon).GetComponent<Image>().gameObject.SetActive(!unLocked);
    }

    private void AddLine(Transform stage, bool isHolderLineH, bool isHolderLineV)
    {
        if (isHolderLineH)
        {
            var childH = Instantiate(line_h, stage);
            childH.transform.SetSiblingIndex(0);
        }
        if (isHolderLineV)
        {
            var childL = Instantiate(line_v, stage);
            if (isHolderLineH) childL.transform.SetSiblingIndex(1);
            else childL.transform.SetSiblingIndex(0);
        }
    }

    private void SetTutorialFirstStage()
    {
        Transform firstStage = contentStage.GetChild(0);
        Instantiate(tutorial, firstStage);
        firstStage.Find(keyStageIndex).GetComponent<Text>().text = "";
    }

    private void SetLevelLabel(Transform stage, int index)
    {
        stage.Find(keyStageIndex).GetComponent<Text>().text = index.ToString();
    }

    private void SetEvent(Transform stage, int stageLevel, bool unLocked = true)
    {
        if (!unLocked) return;
        stage.GetComponent<Button>().onClick.AddListener(delegate { SceneManager.LoadScene(1); GetStageSelectBy(stageLevel); });
    }

    private void GetStageSelectBy(int level)
    {
        StageSelected = level;
    }

    private void SetStarIcon(Transform stage, int amountStar, bool isUnlock = false)
    {
        Transform star = stage.Find(keyStarIcon);

        if (!isUnlock) 
        {
            star.gameObject.SetActive(isUnlock);
            return;
        } 


        star.gameObject.SetActive(isUnlock);
        for (int i = 0; i < star.childCount; i++)
        {
            if(i <= amountStar)
            {
                star.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                star.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    private void SetInfoByFormat(int stageUnlocked)
    {
        stageUnlockedText.text = "STAGE " + stageUnlocked;
    }

    public void ResetStage()
    {
        SetUnlocked(allStage);
        SetRandomStar(allStage);
        ResetStageUI();

        SaveData.SaveStage(allStage);
    }

    private void ResetStageUI()
    {
        foreach (Transform stage in contentStage)
        {
            Stage stageData = GetStageBy(stage.GetSiblingIndex());
            SetUnlocked(stage, stageData.UnLocked);
            SetStarIcon(stage, stageData.AmountStar, stageData.UnLocked);
            SetEvent(stage,stageData.Level, stageData.UnLocked);
        }
    }

    private Stage GetStageBy(int indexContent)
    {
        //When Initialize Stage, indexContent start form 1
        int offset = 1;
        Stage stage = allStage.FirstOrDefault(i => i.IndexContent == (indexContent + offset));
        return stage;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
