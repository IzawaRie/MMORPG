using Assets.Scripts.Managers;
using Battle;
using Common.Battle;
using Models;
using SkillBridge.Message;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISkillSlot : MonoBehaviour,IPointerClickHandler {

	public Image icon;
	public Image overlay;
	public Text cdText;
	Skill skill;

	float overlaySpeed = 0;
	float cdRemain = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(this.skill == null) return;
		if(this.skill.CD > 0)
		{
			if(!overlay.enabled) overlay.enabled = true;
			if(!cdText.enabled) cdText.enabled = true;

			overlay.fillAmount = this.skill.CD / this.skill.Define.CD;
			this.cdText.text = ((int)Math.Ceiling(this.skill.CD)).ToString();
		}
		else
		{
			if(overlay.enabled)overlay.enabled = false;
			if(this.cdText.enabled) this.cdText.enabled = false;
		}
	}


    public void OnPointerClick(PointerEventData eventData)
    {
        if (this.skill.Define.CastTarget == Common.Battle.TargetType.Position)
        {
            TargetSelector.ShowSelector(User.Instance.CurrentCharacter.position, this.skill.Define.CastRange, this.skill.Define.AOERange, OnPositionSelected);
            return;
        }
        CastSkill();
    }

    public void CastSkill()
    {
		SkillResult result = this.skill.CanCast(BattleManager.Instance.CurrentTarget);

        switch(result)
		{
			case SkillResult.InvalidTarget:
				MessageBox.Show("技能:[" + this.skill.Define.Name + "]目标无效");
				return;
			case SkillResult.OutOfMp:
                MessageBox.Show("技能:[" + this.skill.Define.Name + "]MP 不足");
                return;
			case SkillResult.CoolDown:
                //MessageBox.Show("技能:[" + this.skill.Define.Name + "]正在冷却");
                return;
            case SkillResult.OutOfRange:
                MessageBox.Show("技能:[" + this.skill.Define.Name + "]目标超出释放范围");
                return;
        }

		BattleManager.Instance.CastSkill(this.skill);
    }

	public void OnPositionSelected(Vector3 pos)
	{
		BattleManager.Instance.CurrentPosition = GameObjectTool.WorldToLogicN(pos);
		this.CastSkill();
	}

    internal void SetSkill(Skill value)
	{
		this.skill = value;
		if (this.icon != null)
		{
			this.icon.overrideSprite = Resloader.Load<Sprite>(this.skill.Define.Icon);
			this.icon.SetAllDirty();
		}
	}
}
