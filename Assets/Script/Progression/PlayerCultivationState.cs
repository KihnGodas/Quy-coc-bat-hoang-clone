using UnityEngine;

public sealed class PlayerCultivationState : MonoBehaviour
{
    [SerializeField] private CultivationRealm playerRealm = CultivationRealm.LuyenKhi;
    [SerializeField] private CultivationRealm spellRealm = CultivationRealm.LuyenKhi;

    public CultivationRealm PlayerRealm => playerRealm;
    public CultivationRealm SpellRealm => spellRealm;
    public bool CanUseUltimate => playerRealm >= CultivationRealm.KimDan && spellRealm >= CultivationRealm.KimDan;

    public void SetRealms(CultivationRealm newPlayerRealm, CultivationRealm newSpellRealm)
    {
        playerRealm = newPlayerRealm;
        spellRealm = newSpellRealm;
    }

    public void SetRealm(CultivationRealm realm)
    {
        SetRealms(realm, realm);
    }
}
