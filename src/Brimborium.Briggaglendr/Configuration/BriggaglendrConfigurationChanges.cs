namespace Brimborium.Briggaglendr.Configuration;

public class BGConfigurationChanges {
    private readonly BGConfiguration _Current;
    private BGConfiguration _Next;

    public BGConfigurationChanges(
        BGConfiguration current,
        BGConfiguration next
        ) {
        this._Current = current;
        this._Next = next;
    }

    public BGConfiguration Next => this._Next;

    public BGConfiguration Current => this._Current;

    public List<IBGConfigurationChange> GetChanges() {
        List<IBGConfigurationChange> result = new();
        {
            var dictCurrent = this._Current.Cluster;
            var dictNext = this._Next.Cluster;
            var keys = new HashSet<string>(dictCurrent.Keys.Concat(dictNext.Keys));
            foreach (var key in keys) {
                _ = dictCurrent.TryGetValue(key, out var valueCurrent);
                _ = dictNext.TryGetValue(key, out var valueNext);
                if (valueCurrent is null && valueNext is null) {
                    continue;
                }
                if (valueCurrent is null && valueNext is { }) {
                    result.Add(new BGClusterConfigurationChange(key, valueCurrent, valueNext, true));
                    continue;
                }
                if (valueCurrent is { } && valueNext is null) {
                    result.Add(new BGClusterConfigurationChange(key, valueCurrent, valueNext, true));
                    continue;
                }
                if (valueCurrent is { } && valueNext is { }) {
                    var isEqual = valueCurrent.Equals(valueNext);
                    if (isEqual) {
                        if (ReferenceEquals(valueCurrent, valueNext)) {
                            //
                        } else {
                            this._Next = this._Next with {
                                Cluster = this._Next.Cluster.SetItem(key, valueCurrent)
                            };
                        }
                    }
                    result.Add(new BGClusterConfigurationChange(key, valueCurrent, valueNext, isEqual));
                    continue;
                }
            }
        }
        return result;
    }
}
