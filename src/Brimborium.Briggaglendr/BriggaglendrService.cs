using Brimborium.Briggaglendr.Configuration;

namespace Microsoft.AspNetCore.Builder;

public class BriggaglendrService : IDisposable {
    private BGConfiguration _Configuration;
    private IDisposable? _DisposeMonitorOnChange;
    private BriggaglendrOptions _CurrentOptionValue;
    private int _IsInitialized;

    public BriggaglendrService(
        IOptionsMonitor<BriggaglendrOptions> monitor
        ) {
        this._Configuration = BGConfiguration.Empty;

        this._DisposeMonitorOnChange = monitor.OnChange(
            (optionsValue, name) => {
                this._CurrentOptionValue = monitor.CurrentValue;
                if (0 < this._IsInitialized) {
                    this.UseCurrentOptionValue();
                }
            });
        this._CurrentOptionValue = monitor.CurrentValue;
    }

    public BriggaglendrEventReceiver EventReceiver { get; } = new();

    public void Dispose() {
        this._IsInitialized = -1;
        using (var d = this._DisposeMonitorOnChange) {
            this._DisposeMonitorOnChange = default;
        }
    }

    public void Initialize() {
        //
        if (this._IsInitialized == 0) {
            this._IsInitialized = 1;
        }
    }

    public List<BriggaglendrOptions> ListOptions { get; } = new();

    public bool UseCurrentOptionValue() {
        List<BriggaglendrOptions> listOptions = new();
        listOptions.Add(this._CurrentOptionValue);
        listOptions.AddRange(this.ListOptions);
        var nextConfiguration = BGConfigurationConverter.ConvertConfiguration(listOptions);
        BGConfigurationChanges changes = new(this._Configuration, nextConfiguration);
        var result = changes.GetChanges();
        if (0 == result.Count) {
            return false;
        } else {
            this._Configuration = changes.Next;
            foreach (var change in result) {
                change.OnChanged(this.EventReceiver);
            }
            return true;
        }

        // verify this._CurrentOptionValue
        // and 
        //      
        // this._Configuration
    }

    public BGConfiguration Configuration => this._Configuration;
}

