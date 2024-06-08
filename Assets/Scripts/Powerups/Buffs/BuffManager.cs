using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flamenccio.Powerup.Buff
{
    /// <summary>
    /// Manages the player's buffs.
    /// </summary>
    public class BuffManager : MonoBehaviour
    {
        public List<BuffBase> Buffs { get => buffs; }
        private List<BuffBase> buffs = new();
        private Action buffUpdate;
        private PlayerAttributes playerAttributes;

        private void Start()
        {
            playerAttributes = GetComponent<PlayerAttributes>();
        }

        private void Update()
        {
            buffUpdate?.Invoke();
        }

        #region add buff methods

        /// <summary>
        /// Attempts to add a buff to the buff list given it's Type.
        /// <para>Will not add anything if the given Type is not valid.</para>
        /// </summary>
        /// <param name="buffType">Type of the buff.</param>
        public void AddBuff(Type buffType)
        {
            if (!buffType.IsSubclassOf(typeof(BuffBase))) return;

            BuffBase buffInstance;

            if (buffType.IsSubclassOf(typeof(ConditionalBuff)))
            {
                Action<List<PlayerAttributes.Attribute>> x = LevelBuff;
                buffInstance = Activator.CreateInstance(buffType, new object[] { playerAttributes, x }) as BuffBase;
            }
            else
            {
                buffInstance = Activator.CreateInstance<BuffBase>();
            }

            AddBuff(buffInstance);
        }

        private void AddBuff(BuffBase b)
        {
            int x = FindBuff(b);

            if (x < 0)
            {
                buffs.Add(b); // if there is no existing duplciate, add it

                if (b is ConditionalBuff)
                {
                    buffUpdate += (b as ConditionalBuff).Run;
                }
            }
            else
            {
                buffs[x].LevelUp(); // if there is an existing duplicate level up
            }

            foreach (PlayerAttributes.Attribute a in b.GetAffectedAttributes())
            {
                playerAttributes.RecompileBonus(a, buffs);
            }
        }
        #endregion

        #region remove buff methods
        private bool RemoveBuff(BuffBase b)
        {
            int x = FindBuff(b);

            if (x < 0)
            {
                return false;
            }

            if (b is ConditionalBuff)
            {
                buffUpdate -= (b as ConditionalBuff).Run;
            }

            buffs[x].OnDestroy();
            buffs.RemoveAt(x);
            buffs.Sort((BuffBase a, BuffBase b) => a.Level < b.Level ? -1 : 1); // basically re-sort the list based on level: higher level buffs will be placed at the top.

            foreach (PlayerAttributes.Attribute a in b.GetAffectedAttributes())
            {
                playerAttributes.RecompileBonus(a, buffs);
            }

            return true;
        }

        /// <summary>
        /// Attempts to remove a buff of given Type from the buff list.
        /// </summary>
        /// <param name="buffType">The Type of the buff.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool RemoveBuff(Type buffType)
        {
            if (!buffType.IsSubclassOf(typeof(BuffBase))) return false;

            int x = FindBuff(buffType);

            if (x < 0) return false;

            BuffBase remove = buffs[x];

            if (remove is ConditionalBuff)
            {
                buffUpdate -= (remove as ConditionalBuff).Run;
            }

            remove.OnDestroy();
            buffs.RemoveAt(x);
            buffs.Sort((BuffBase a, BuffBase b) => a.Level < b.Level ? -1 : 1); // basically re-sort the list based on level: higher level buffs will be placed at the top.

            foreach (PlayerAttributes.Attribute a in remove.GetAffectedAttributes())
            {
                playerAttributes.RecompileBonus(a, buffs);
            }

            return true;
        }
        #endregion

        #region find buff methods
        /// <summary>
        /// Finds a buff from <b>buffs</b> and returns its index in the list. -1 if it doesn't exist.
        /// </summary>
        private int FindBuff(BuffBase b)
        {
            return FindBuff(b.Name);
        }

        private int FindBuff(string buffName)
        {
            int i = 0;
            foreach (BuffBase bb in buffs)
            {
                if (bb.Name.Equals(buffName))
                {
                    return i;
                }
                i++;
            }
            return -1;
        }

        private int FindBuff(Type buffType)
        {
            int i = 0;
            foreach (BuffBase bb in buffs)
            {
                if (bb.GetType() == buffType)
                {
                    return i;
                }
                i++;
            }
            return -1;
        }
        #endregion

        private void LevelBuff(List<PlayerAttributes.Attribute> affectedAttributes)
        {
            foreach (var attribute in affectedAttributes)
            {
                playerAttributes.RecompileBonus(attribute, Buffs);
            }
        }
    }
}
