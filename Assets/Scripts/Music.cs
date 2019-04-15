using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    // From Unity docs:
// Basic demonstration of a music system that uses PlayScheduled to preload and sample-accurately
// stitch two AudioClips in an alternating fashion.

public class Music : MonoBehaviour
{

    private string mode = "";

    public AudioClip introVocal;
    public AudioClip mainVocal;
    public AudioClip bridgeVocal;
    public AudioClip endingVocal;

    public AudioClip introSea;
    public AudioClip mainSea;
    public AudioClip bridgeSea;
    public AudioClip endingSea;

    public AudioClip introNorth;
    public AudioClip mainNorth;
    public AudioClip bridgeNorth;
    public AudioClip endingNorth;

    public AudioClip introSouth;
    public AudioClip mainSouth;
    public AudioClip bridgeSouth;
    public AudioClip endingSouth;

    public AudioClip introEast;
    public AudioClip mainEast;
    public AudioClip bridgeEast;
    public AudioClip endingEast;

    public AudioClip introWest;
    public AudioClip mainWest;
    public AudioClip bridgeWest;
    public AudioClip endingWest;

    public AudioSource[] audioSourcesVocal = new AudioSource[2];
    public AudioSource[] audioSourcesSea = new AudioSource[2];
    public AudioSource[] audioSourcesNorth = new AudioSource[2];
    public AudioSource[] audioSourcesSouth = new AudioSource[2];
    public AudioSource[] audioSourcesEast = new AudioSource[2];
    public AudioSource[] audioSourcesWest = new AudioSource[2];

    private double nextEventTime;
    private int flip = 0;

    private bool running = false;

    void Start()
    {
        nextEventTime = AudioSettings.dspTime;
        running = true;
    }

    void Update()
    {
        if (!running)
        {
            return;
        }

        double time = AudioSettings.dspTime;

        if (time + 2.0f > nextEventTime)
        {
            // We are now approx. 1 second before the time at which the sound should play,
            // so we will schedule it now in order for the system to have enough time
            // to prepare the playback at the specified time. This may involve opening
            // buffering a streamed file and should therefore take any worst-case delay into account.

            switch(mode)
            {
                case "intro":
                    mode = "main";
                    audioSourcesVocal[flip].clip = mainVocal;
                    audioSourcesSea[flip].clip = mainSea;
                    audioSourcesNorth[flip].clip = mainNorth;
                    audioSourcesSouth[flip].clip = mainSouth;
                    audioSourcesEast[flip].clip = mainEast;
                    audioSourcesWest[flip].clip = mainWest;
                    break;

                case "main":
                    mode = "bridge";
                    audioSourcesVocal[flip].clip = bridgeVocal;
                    audioSourcesSea[flip].clip = bridgeSea;
                    audioSourcesNorth[flip].clip = bridgeNorth;
                    audioSourcesSouth[flip].clip = bridgeSouth;
                    audioSourcesEast[flip].clip = bridgeEast;
                    audioSourcesWest[flip].clip = bridgeWest;
                    break;

                case "bridge":
                    mode = "main";
                    audioSourcesVocal[flip].clip = mainVocal;
                    audioSourcesSea[flip].clip = mainSea;
                    audioSourcesNorth[flip].clip = mainNorth;
                    audioSourcesSouth[flip].clip = mainSouth;
                    audioSourcesEast[flip].clip = mainEast;
                    audioSourcesWest[flip].clip = mainWest;
                    break;

                case "end":
                    mode = "";
                    audioSourcesVocal[flip].clip = endingVocal;
                    audioSourcesSea[flip].clip = endingSea;
                    audioSourcesNorth[flip].clip = endingNorth;
                    audioSourcesSouth[flip].clip = endingSouth;
                    audioSourcesEast[flip].clip = endingEast;
                    audioSourcesWest[flip].clip = endingWest;
                    running = false;
                    break;

                default:
                    mode = "intro";
                    audioSourcesVocal[flip].clip = introVocal;
                    audioSourcesSea[flip].clip = introSea;
                    audioSourcesNorth[flip].clip = introNorth;
                    audioSourcesSouth[flip].clip = introSouth;
                    audioSourcesEast[flip].clip = introEast;
                    audioSourcesWest[flip].clip = introWest;
                    break;
            }

            audioSourcesVocal[flip].PlayScheduled(nextEventTime);
            audioSourcesSea[flip].PlayScheduled(nextEventTime);
            audioSourcesNorth[flip].PlayScheduled(nextEventTime);
            audioSourcesSouth[flip].PlayScheduled(nextEventTime);
            audioSourcesEast[flip].PlayScheduled(nextEventTime);
            audioSourcesWest[flip].PlayScheduled(nextEventTime);

            nextEventTime += audioSourcesSea[flip].clip.length;

            // Flip between two audio sources so that the loading process of one does not interfere with the one that's playing out
            flip = 1 - flip;
        }
    }
}
