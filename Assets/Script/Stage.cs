[System.Serializable]
public class Stage 
{
    private int level;
    private bool unLocked;
    private int amountStar;
    private int indexContent;
    private bool isHoderLineH;
    private bool isHoderLineV;

    public int Level { get => level; set => level = value; }
    public bool UnLocked { get => unLocked; set => unLocked = value; }
    public int AmountStar { get => amountStar; set => amountStar = value; }
    public int IndexContent { get => indexContent; set => indexContent = value; }
    public bool IsHoderLineH { get => isHoderLineH; set => isHoderLineH = value; }
    public bool IsHoderLineV { get => isHoderLineV; set => isHoderLineV = value; }

    public Stage(int indexContent)
    {
        IndexContent = indexContent;
        IsHoderLineH = false;
        IsHoderLineV = false;
    }
}
