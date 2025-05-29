using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    public static UIInventory Instance { get; private set; }

    public ItemSlot[] slots;

    public GameObject inventoryWindow;
    public Transform slotPanel;
    public Transform dropPosition;

    [Header("Select Item")]
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedStatName;
    public TextMeshProUGUI selectedStatValue;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unequipButton;
    public GameObject dropButton;

    [System.Serializable]
    public class StartingItem
    {
        public ItemData itemData;
        public int quantity = 1;
    }

    [Header("Starting Items")]
    public StartingItem[] startingItems; // 시작 아이템 배열

    private PlayerController controller;
    private PlayerCondition condition;

    ItemData selectedItem;
    int selectedItemIndex = 0;

    int curEquipIndex;

    private void Start()
    {
        Instance = this;

        controller = CharacterManager.Instance.Player.controller;
        condition = CharacterManager.Instance.Player.condition;
        dropPosition = CharacterManager.Instance.Player.dropPosition;

        controller.inventory += Toggle;
        CharacterManager.Instance.Player.addItem += AddItem;

        inventoryWindow.SetActive(false);
        slots = new ItemSlot[slotPanel.childCount];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index = i;
            slots[i].inventory = this;
        }

        ClearSelctedItemWindow();

        // 시작 아이템 추가
        AddStartingItems();
    }

    private void AddStartingItems()
    {
        foreach (StartingItem startingItem in startingItems)
        {
            if (startingItem.itemData != null)
            {
                AddItemToInventory(startingItem.itemData, startingItem.quantity);
            }
        }
    }

    private void AddItemToInventory(ItemData data, int quantity)
    {
        int remainingQuantity = quantity;

        // 중복 가능한 아이템인 경우 기존 스택에 추가
        if (data.canStack)
        {
            for (int i = 0; i < slots.Length && remainingQuantity > 0; i++)
            {
                if (slots[i].item == data && slots[i].quantity < data.maxStackAmount)
                {
                    int addableAmount = Mathf.Min(remainingQuantity, data.maxStackAmount - slots[i].quantity);
                    slots[i].quantity += addableAmount;
                    remainingQuantity -= addableAmount;
                }
            }
        }

        // 빈 슬롯에 아이템 추가
        for (int i = 0; i < slots.Length && remainingQuantity > 0; i++)
        {
            if (slots[i].item == null)
            {
                int addAmount = data.canStack ? Mathf.Min(remainingQuantity, data.maxStackAmount) : 1;
                slots[i].item = data;
                slots[i].quantity = addAmount;
                remainingQuantity -= addAmount;
            }
        }

        UpdateUI();
    }

    void ClearSelctedItemWindow()
    {
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;

        useButton.SetActive(false);
        equipButton.SetActive(false);
        unequipButton.SetActive(false);
        dropButton.SetActive(false);
    }

    public void Toggle()
    {
        if (IsOpen())
        {
            inventoryWindow.SetActive(false);
        }
        else
        {
            inventoryWindow.SetActive(true);
        }
    }

    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    void AddItem()
    {
        ItemData data = CharacterManager.Instance.Player.itemData;


        //아이템이 중복 가능한지 canStack
        if (data.canStack)
        {
            ItemSlot slot = GetItemStack(data);
            if (slot != null)
            {
                slot.quantity++;
                UpdateUI();
                CharacterManager.Instance.Player.itemData = null;
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();

        if (emptySlot != null)
        {
            emptySlot.item = data;
            emptySlot.quantity = 1;
            UpdateUI();
            CharacterManager.Instance.Player.itemData = null;
            return;
        }

        ThrowItem(data);
        CharacterManager.Instance.Player.itemData = null;
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                slots[i].Set();
            }
            else
            {
                slots[i].Clear();
            }
        }
    }

    ItemSlot GetItemStack(ItemData data)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == data && slots[i].quantity < data.maxStackAmount)
            {
                return slots[i];
            }
        }
        return null;
    }

    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                return slots[i];
            }
        }
        return null;
    }

    void ThrowItem(ItemData data)
    {
        Instantiate(data.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360));
    }

    public void SelectItem(int index)
    {
        if (slots[index].item == null) return;

        selectedItem = slots[index].item;
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.displayName;
        selectedItemDescription.text = selectedItem.description;

        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;

        for (int i = 0; i < selectedItem.consumables.Length; i++)
        {
            selectedStatName.text += selectedItem.consumables[i].type.ToString() + "\n";
            selectedStatValue.text += selectedItem.consumables[i].value.ToString() + "\n";
        }

        useButton.SetActive(selectedItem.type == ItemType.Consumable);
        equipButton.SetActive(selectedItem.type == ItemType.Equipable && !slots[index].equipped);
        unequipButton.SetActive(selectedItem.type == ItemType.Equipable && slots[index].equipped);
        dropButton.SetActive(true);
    }

    public void OnUseButton()
    {
        if (selectedItem.type == ItemType.Consumable)
        {
            for (int i = 0; i < selectedItem.consumables.Length; i++)
            {
                switch (selectedItem.consumables[i].type)
                {
                    case ConsumableType.Health:
                        condition.Heal(selectedItem.consumables[i].value);
                        break;
                    case ConsumableType.Hunger:
                        condition.Eat(selectedItem.consumables[i].value);
                        break;
                    case ConsumableType.Arrow:
                        Debug.Log("화살은 사용할 수 없습니다. 자동으로 소비됩니다.");
                        return; // 화살은 사용하지 않고 자동 소비만
                }
            }
            RemoveSelectedItem();
        }
    }

    public void OnDropButton()
    {
        ThrowItem(selectedItem);
        RemoveSelectedItem();
    }

    void RemoveSelectedItem()
    {
        slots[selectedItemIndex].quantity--;

        if (slots[selectedItemIndex].quantity <= 0)
        {
            selectedItem = null;
            slots[selectedItemIndex].item = null;
            selectedItemIndex = -1;
            ClearSelctedItemWindow();
        }

        UpdateUI();
    }

    public void OnEquipButton()
    {
        if (slots[curEquipIndex].equipped)
        {
            UnEquip(curEquipIndex);
        }

        slots[selectedItemIndex].equipped = true;
        curEquipIndex = selectedItemIndex;
        CharacterManager.Instance.Player.equip.EquipNew(selectedItem);
        UpdateUI();

        SelectItem(selectedItemIndex);
    }

    void UnEquip(int index)
    {
        slots[index].equipped = false;
        CharacterManager.Instance.Player.equip.UnEquip();
        UpdateUI();

        if (selectedItemIndex == index)
        {
            SelectItem(selectedItemIndex);
        }
    }

    public void OnUnEquipBotton()
    {
        UnEquip(selectedItemIndex);
    }

    // 화살 소비 함수 (EquipBow에서 호출)
    public bool ConsumeArrow(ItemData arrowItemData = null, int amount = 1)
    {
        // 인벤토리에서 화살 아이템 찾기 (Arrow 타입의 Consumable 아이템)
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                // Arrow 타입의 Consumable 아이템인지 확인
                bool isArrowItem = (slots[i].item.type == ItemType.Consumable &&
                                   slots[i].item.consumables != null &&
                                   slots[i].item.consumables.Length > 0 &&
                                   slots[i].item.consumables[0].type == ConsumableType.Arrow);

                if (isArrowItem && slots[i].quantity >= amount)
                {
                    slots[i].quantity -= amount;

                    // 수량이 0이 되면 슬롯을 비움
                    if (slots[i].quantity <= 0)
                    {
                        slots[i].item = null;
                        slots[i].quantity = 0;
                    }

                    UpdateUI();
                    return true; // 소비 성공
                }
            }
        }
        return false; // 소비 실패 (화살이 없음)
    }

    // 화살 아이템 개수 확인
    public int GetArrowCount(ItemData arrowItemData = null)
    {
        int totalCount = 0;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                // Arrow 타입의 Consumable 아이템인지 확인
                bool isArrowItem = (slots[i].item.type == ItemType.Consumable &&
                                   slots[i].item.consumables != null &&
                                   slots[i].item.consumables.Length > 0 &&
                                   slots[i].item.consumables[0].type == ConsumableType.Arrow);

                if (isArrowItem)
                {
                    totalCount += slots[i].quantity;
                }
            }
        }

        return totalCount;
    }
}
