using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MinaSignerNet
{
    public class GroupProjective
    {
        public BigInteger X { get; set; }
        public BigInteger Y { get; set; }
        public BigInteger Z { get; set; }

        public GroupProjective()
        {

        }

        public GroupProjective(GroupAffine groupAffine)
        {
            if (groupAffine.Infinity)
            {
                X = Constants.ProjectiveZero.X;
                Y = Constants.ProjectiveZero.Y;
                Z = Constants.ProjectiveZero.Z;
            }

            X = groupAffine.X;
            Y = groupAffine.Y;
            Z = BigInteger.One;
        }

        public GroupProjective(Group group)
        {
            X = group.X;
            Y = group.Y;
            Z = BigInteger.One;
        }

        public Group ToGroup()
        {
            GroupAffine affine = this.ProjectiveToAffine(Constants.P);
            if (affine.Infinity)
                throw new Exception("Group.fromProjective: point is infinity");
            return affine;
        }

    }
}
