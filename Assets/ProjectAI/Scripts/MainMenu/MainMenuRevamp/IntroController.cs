using System.Collections;
using TMPro;
using UnityEngine;
using Zenject;
namespace Assets.ProjectAI.Scripts.MainMenu 
{ 
    public class IntroController : MonoBehaviour 
    { 
        [SerializeField] private TMP_Text _introText; 
        [SerializeField] private float _textSpeed = 0.04f; //Typing Speed
        [SerializeField] private float _lineDelay = 1.0f; //Delay between lines 
        [SerializeField] private GameObject _smokeEffect; 
        [Inject] private ISceneManager _sceneManager; 
        public bool IsIntroComplete { get; private set; } = false; 
        public bool SkipIntro { get; set; } = false; 
        private readonly string[] _lines = { 
            "The dungeon discovered in search of gold,", 
            "it opened its doors to treasures untold,", 
            "but danger lurked past every door,", 
            "and none could reach the lower floors.", 
            "", 
            "Brave or Foolish you may be,", 
            "To venture far beyond and deep.", 
            "But this nugget of knowledge you should keep:", 
            "For those who wander, wander deep." }; 
        private void Start() 
        { 
            //_smokeEffect.SetActive(true); 
            StartCoroutine(PlayIntro()); 
        } 
        private void Update() 
        { 
            if (SkipIntro && !IsIntroComplete) 
            { 
                StopAllCoroutines(); 
                _sceneManager.FadeToBlack(); 
                IsIntroComplete = true; 
            } 
        } 
        private IEnumerator PlayIntro() 
        { 
            _introText.text = ""; 
            foreach(var line in _lines) 
            { 
                yield return StartCoroutine(TypeLine(line)); 
                yield return new WaitForSeconds(_lineDelay); 
                //_introText.text += "\n"; 
                //Add a new line after each line
            } 
            yield return new WaitForSeconds(2.0f); 
            //Wait before transitioning 
            //_sceneManager.FadeToBlack();
            IsIntroComplete = true;
            gameObject.SetActive(false);
        } 
        private IEnumerator TypeLine(string line) 
        { 
            string previousText = _introText.text;
            _introText.text = previousText + "\n";
            for (int i = 0; i < line.Length; i++)
            {
                _introText.text = previousText + "\n" + line.Substring(0, i + 1);
                yield return new WaitForSeconds(_textSpeed);
            }

            _introText.text = previousText + "\n" + line;
        } 
    } 
}