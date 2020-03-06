using System.Collections.Generic;
using System.Threading.Tasks;

public class BattleAnimShader : BattleAnim {

    protected new BattleAnimShaderData data;

    public BattleAnimShader(BattleAnimShaderData data) : base(data) {
        this.data = data;
    }

    protected override Task PlayInternalAsync(BattleView view, List<Unit> targets) {
        throw new System.NotImplementedException();
    }
}
