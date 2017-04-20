
/// <summary>
/// 비밀거래 완료
/// </summary>
public class AchieveSecretBusiness : AchieveBase
{

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.secretBusiness.onOpenSecretBoxCallback += Listener;
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.secretBusiness.onOpenSecretBoxCallback -= Listener;
        }
    }

    void Listener(int cardIndex)
    {
        achieveAccumulate++;
    }
}
