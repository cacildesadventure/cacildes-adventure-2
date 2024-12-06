namespace AF
{
    using System;
    using System.Collections.Generic;

    public static class TutorialUtils
    {

        public static List<Tutorial.RenderingElement> GetRenderingElements(Tutorial.TutorialStep tutorialStep, string word)
        {
            List<Tutorial.RenderingElement> tutorialStepContent = new();

            findWord(word, (wordToAdd) =>
            {
                tutorialStepContent.Add(new Tutorial.RenderingElement() { text = wordToAdd });
                return true;
            },
            (buttonIndexToAdd) =>
            {
                int.TryParse(buttonIndexToAdd.ToString(), out int buttonIndex);
                tutorialStepContent.Add(new Tutorial.RenderingElement() { button = tutorialStep.actionButtons[buttonIndex] });
                return true;
            });

            return tutorialStepContent;
        }

        public static void findWord(string word, Func<string, bool> onWordAdded, Func<string, bool> onButtonAdd)
        {
            string tempWord = "";

            for (int i = 0; i < word.Length; i++)
            {
                // We found code bracket, add previous constructed word to the list
                if (word[i] == '{')
                {
                    string buttonIndexString = "";

                    onWordAdded(tempWord);
                    tempWord = "";

                    for (int j = i + 1; j < word.Length; j++)
                    {
                        // We add the action button, then jump the sequence to continue the phrase
                        if (word[j] == '}')
                        {
                            onButtonAdd(buttonIndexString);

                            i = j;
                            break;
                        }
                        else
                        {
                            buttonIndexString += word[j].ToString();
                        }
                    }
                }
                else
                {
                    tempWord += word[i];
                }
            }

            if (tempWord.Length > 0)
            {
                onWordAdded(tempWord);
            }
        }

    }
}
