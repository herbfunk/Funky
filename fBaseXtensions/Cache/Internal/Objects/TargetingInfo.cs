namespace fBaseXtensions.Cache.Internal.Objects
{
    public class TargetingInfo
    {
        private readonly CacheUnit _unit;

        private float _radiusDistance = -1;
        public float RadiusDistance
        {
            get
            {
                if (_radiusDistance == -1)
                    _radiusDistance = _unit.RadiusDistance;

                return _radiusDistance;
            }
        }

        private int _intersectingUnits = -1;
        public int IntersectingUnits
        {
            get
            {
                if (_intersectingUnits == -1)
                    _intersectingUnits = ObjectCache.Objects.TotalIntersectingUnits(_unit.Position, 8f);

                return _intersectingUnits;
            }
        }

        public TargetingInfo()
        {
            _unit = new CacheUnit(ObjectCache.FakeCacheObject);
        }
        public TargetingInfo(CacheUnit unit)
        {
            _unit = unit;
        }
    }
}