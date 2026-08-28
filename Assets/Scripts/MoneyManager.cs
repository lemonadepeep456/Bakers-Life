using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    public int money;
    public TMP_Text moneyText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateUI();

        Debug.Log("Money: $" + money);
    }

    void UpdateUI()
    {
        moneyText.text = "$" + money;
    }
}