using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QTEPack
{
    public class QTE_CircleHit : QuickTimeEvent
    {
        [Header("DBD Style Settings")]
        [Tooltip("How many consecutive hits are needed to pass the QTE?")]
        [SerializeField] private int requiredHitsToWin = 6; 
        
        // NEW: Sound slots!
        [SerializeField] private AudioClip hitSound; 
        [SerializeField] private AudioClip failSound; 

        [Header("Original Settings")]
        [SerializeField] private float[] HitAreaScaleByDifficulty;
        [SerializeField] private float[] CursorSpeedByDifficulty;
        [SerializeField] private Image hitAreaImage;
        [SerializeField] private TMP_Text resultText;
        [SerializeField] private RectTransform cursor;

        private float hitAreaRotationZ;
        private float cursorRotationZ;
        private float minCursorRotToWin;
        private float maxCursorRotToWin;
        private QTEHitResult done;

        // Variables to track our DBD mechanics
        private int baseDifficulty; 
        private int currentDifficulty;
        private int currentSuccessfulHits;
        private float moveDirection; 

        public override void ShowQTE(Vector2 position, float scale, int difficulty)
        {
            base.ShowQTE(position, scale, difficulty);

            baseDifficulty = difficulty;
            currentDifficulty = difficulty;
            
            currentSuccessfulHits = 0;
            moveDirection = 1f; 

            GenerateNewHitArea();

            done = QTEHitResult.Playing;
            resultText.text = "";
            StartCoroutine(RunQTE(difficulty));
        }

        private void GenerateNewHitArea()
        {
            hitAreaImage.fillAmount = HitAreaScaleByDifficulty[currentDifficulty];
            
            hitAreaRotationZ = Random.Range(0, 360);
            hitAreaImage.rectTransform.rotation = Quaternion.Euler(0, 0, hitAreaRotationZ);

            hitAreaImage.color = ColorByDifficulty[currentDifficulty];

            var allowedErrorByDifficulty = -350f * HitAreaScaleByDifficulty[currentDifficulty] + 7.5f;
            minCursorRotToWin = hitAreaRotationZ + allowedErrorByDifficulty;
            maxCursorRotToWin = hitAreaRotationZ + 10;
        }

        public IEnumerator RunQTE(int difficulty)
        {
            cursorRotationZ = 0;

            while (done == QTEHitResult.Playing)
            {
                float speed = CursorSpeedByDifficulty[currentDifficulty];
                
                cursorRotationZ -= FixedSpeed(speed) * moveDirection;
                cursor.rotation = Quaternion.Euler(0, 0, cursorRotationZ);

                yield return new WaitForEndOfFrame();
            }

            if (done == QTEHitResult.Win)
            {
                resultText.text = "Success!!!";
                OnSuccess.Invoke();
            }
            else
            {
                resultText.text = "Ups...";
                OnFail.Invoke();
            }
        }

        private void Update()
        {
            if (done == QTEHitResult.Playing)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) 
                {
                    var normalizedCursorZ = cursorRotationZ % 360;

                    var checkA = normalizedCursorZ >= System.Math.Min(minCursorRotToWin, maxCursorRotToWin) && normalizedCursorZ <= System.Math.Max(minCursorRotToWin, maxCursorRotToWin);
                    var checkB = (360 + normalizedCursorZ) >= System.Math.Min(minCursorRotToWin, maxCursorRotToWin) && (360 + normalizedCursorZ) <= System.Math.Max(minCursorRotToWin, maxCursorRotToWin);

                    if (checkA || checkB)
                    {
                        // NEW: Play the success sound using your SoundFXManager!
                        if (hitSound != null && SoundFXManager.instance != null)
                        {
                            SoundFXManager.instance.PlaySoundFXClip(hitSound, transform, 0.8f);
                        }

                        currentSuccessfulHits++;
                        
                        if (currentSuccessfulHits >= requiredHitsToWin)
                        {
                            done = QTEHitResult.Win;
                        }
                        else
                        {
                            int hitsRemaining = requiredHitsToWin - currentSuccessfulHits;

                            if (hitsRemaining == 2)
                            {
                                currentDifficulty = Mathf.Clamp(baseDifficulty + 1, 0, 4);
                            }
                            else if (hitsRemaining == 1)
                            {
                                currentDifficulty = Mathf.Clamp(baseDifficulty + 2, 0, 4);
                            }

                            moveDirection *= -1f; 
                            GenerateNewHitArea();
                        }
                    }
                    else
                    {
                        // NEW: Play the fail sound!
                        if (failSound != null && SoundFXManager.instance != null)
                        {
                            SoundFXManager.instance.PlaySoundFXClip(failSound, transform, 0.8f);
                        }
                        
                        done = QTEHitResult.Fail;
                    }
                }
            }
        }

        private float FixedSpeed(float speed)
        {
            return speed * Time.fixedDeltaTime;
        }

        public override void Hide()
        {
            base.Hide();
            done = QTEHitResult.Playing;
        }
    }
}