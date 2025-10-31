using System;
using System.Collections.Generic;
using FishingGame.GameTime;
using FishingGame.Reeling;
using FishingGame.SaveGame;
using UnityEngine;

namespace FishingGame.FishSystem
{
    [Serializable]
    public struct FishableWeight
    {
        public float MinWeight;
        public float MaxWeight;
    }
    
    /// <summary>
    /// Base type for fishable scriptable objects. Has all common parameters between all fishable objects
    /// </summary>
    public class FishableScriptable : SerializableObject
    {
        public enum EFishingDifficulty
        {
            VeryEasy=1,
            Easy,
            Normal,
            Hard,
            VeryHard,
            NearImpossible
        }
        public Sprite Texture;
        public string Name;
        public FishableWeight WeightRange;
        public EFishingDifficulty Difficulty;
        public string Biography;
        public List<EFishingLocation> LocationsFound;
        public List<ETimeOfDay> TimesFound;
        public ArrowWaveSO ArrowMiniGameBehaviour;
        public SliderSO SliderMiniGameBehaviour;
        
        internal FishableScriptable(int persistentID) : base(persistentID)
        {
        }
    }
}