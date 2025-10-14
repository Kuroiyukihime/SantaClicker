using UnityEngine;
using System.Collections.Generic;

public class ClickerManager : MonoBehaviour
{
    [Header("Clicker Items")]
    public List<ClickerItem> clickerItems;
    public PlayerData playerData;

    [Header("Gingerbread Clicker")]
    public string gingerbreadName = "Gingerbread";
    public double gingerbreadPerClick = 1;
    public double gingerbreadPerSecond = 0;
    
    [Header("Candy Cane Clicker")]
    public string candyCaneName = "Candy Cane";
    public double candyCanePerClick = 1;
    public double candyCanePerSecond = 0;
    
    [Header("Cookie Clicker")]
    public string cookieName = "Cookie";
    public double cookiePerClick = 1;
    public double cookiePerSecond = 0;

    void Start()
    {
        SetupClickerItems();
    }

    // Update is called once per frame
    void Update()
    {
        double delta = Time.deltaTime;
        foreach (var item in clickerItems)
        {
            item.Generate(delta);
        }
    }

    public void HandleClick (string itemName)
    {
        var item = clickerItems.Find(item => item.itemName == itemName);
        if (item != null)
        {
            item.OnClick();
        }
    }

    void SetupClickerItems()
    {
        // Initialize clicker items list
        if (clickerItems == null)
        {
            clickerItems = new List<ClickerItem>();
        }

        // Create Gingerbread clicker item
        CreateClickerItem(gingerbreadName, Currency.GingerBread, gingerbreadPerClick, gingerbreadPerSecond);
        
        // Create Candy Cane clicker item
        CreateClickerItem(candyCaneName, Currency.CandyCane, candyCanePerClick, candyCanePerSecond);
        
        // Create Cookie clicker item
        CreateClickerItem(cookieName, Currency.Cookie, cookiePerClick, cookiePerSecond);
    }

    void CreateClickerItem(string itemName, Currency currency, double perClick, double perSecond)
    {
        // Create a new GameObject for the clicker item
        GameObject itemObject = new GameObject(itemName + " Clicker");
        itemObject.transform.SetParent(transform);
        
        // Add ClickerItem component
        ClickerItem clickerItem = itemObject.AddComponent<ClickerItem>();
        
        // Set up the clicker item properties
        clickerItem.itemName = itemName;
        clickerItem.currency = currency;
        clickerItem.perClick = perClick;
        clickerItem.persecond = perSecond;
        
        // ClickerItem now uses static GameManager for data access
        
        // Add to the clicker items list
        clickerItems.Add(clickerItem);
        
        Debug.Log($"Created {itemName} clicker item with {perClick} per click and {perSecond} per second");
    }
}
