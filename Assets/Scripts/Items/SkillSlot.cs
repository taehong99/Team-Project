using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillSlot : BaseUI, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Slot Data")]
    public PlayerSkillDataSO skillData;
    [SerializeField] bool filled;
    SkillSlots parent;
    Image icon;
    public Image cooldownImage;
    int idx;

    void Start()
    {
        icon = GetUI<Image>("SkillIcon");
        cooldownImage = GetUI<Image>("Cooldown");
    }

    private void Update()
    {
        if (filled)
        {
            cooldownImage.fillAmount = Manager.Game.GetCooldownRatio(skillData.id);
        }
        else
        {
            if(cooldownImage != null)
            {
                cooldownImage.fillAmount = 0;
            }
        }
    }

    public void InstantiateSlot(int idx, SkillSlots parent)
    {
        this.idx = idx;
        this.parent = parent;
        filled = false;
    }

    public void UpdateIcon()
    {
        GetUI<Image>("SkillIcon").sprite = skillData.icon;
        filled = true;
    }

    public void UpdateIcon(Sprite newIcon)
    {
        GetUI<Image>("SkillIcon").sprite = newIcon;
        filled = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!filled)
            return;

        Debug.Log("Begin drag");
        if (parent.dragData == null) // Instantiate if null
            parent.dragData = new SkillSlots.DragData();

        parent.dragData.slot = this;
        parent.dragData.transform = transform;
        icon.transform.SetParent(GetComponentInParent<Canvas>().transform);
        icon.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!filled)
            return;
        Debug.Log("Dragging");
        icon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!filled)
            return;
        Debug.Log("End Drag");
        icon.raycastTarget = true;

        // Item dropped in invalid area
        if (!RectTransformUtility.RectangleContainsScreenPoint((RectTransform)parent.transform, eventData.position))
        {
            icon.transform.SetParent(parent.dragData.transform);
            icon.transform.SetAsFirstSibling();
            icon.transform.localPosition = Vector2.zero;
            return;
        }
        else
        {
            SkillSlot[] slots = parent.inventorySlots;
            for (int i = 0; i < slots.Length; i++)
            {
                RectTransform rect = (RectTransform)slots[i].transform;
                if (RectTransformUtility.RectangleContainsScreenPoint(rect, eventData.position))
                {
                    if (slots[i] == parent.dragData.slot) // Slot is same as start
                    {
                        icon.transform.SetParent(parent.dragData.transform);
                        icon.transform.SetAsFirstSibling();
                        icon.transform.localPosition = Vector2.zero;
                        return;
                    }
                    else // Swap image with slot
                    {
                        Sprite tempSprite = slots[i].icon.sprite;
                        slots[i].icon.sprite = icon.sprite;
                        icon.sprite = tempSprite;
                        if (slots[i].filled) // Filled slot
                        {
                            // slots[i].skillData = dropped slot
                            // skillData = original slot
                            PlayerSkillDataSO tempData = skillData;
                            skillData = slots[i].skillData;
                            slots[i].skillData = tempData;
                            Manager.Game.UpdateSkillSlot(idx, i, tempData);
                        }
                        else
                        {
                            slots[i].skillData = skillData;
                            Manager.Game.UpdateSkillSlot(idx, i, skillData);
                            //Manager.Game.UpdateSkillSlot(i, skillData);
                            //Manager.Game.UpdateSkillSlot(idx, null);
                            skillData = null;
                            filled = false;
                            slots[i].filled = true;
                        }
                    }
                }
            }
        }
        icon.transform.SetParent(parent.dragData.transform);
        icon.transform.SetAsFirstSibling();
        icon.transform.localPosition = Vector2.zero;
    }
}    
        
    

    