
namespace AF
{
    using System.Collections;
    using System.Linq;

    public class EV_ShowTutorial : EventBase
    {
        public TutorialManager tutorialManager;
        public UIDocumentTutorial uIDocumentTutorial;

        public Tutorial tutorial;

        public override IEnumerator Dispatch()
        {
            yield return null;

            tutorialManager.SetTutorial(tutorial);
        }
    }
}
