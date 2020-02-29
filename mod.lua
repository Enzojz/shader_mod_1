function data()
    return {
        info = {
            minorVersion = 1,
            severityAdd = "NONE",
            severityRemove = "NONE",
            minorVersion = 1,
            name = _("title"),
            description = _("description"),
            visible = true,
            authors = {
                {
                    name = 'Enzojz',
                    role = 'CREATOR',
                    tfnetId = 27218,
                },
            },
            tags = {"Shader"},
        },
        runFn = function(_)
            game.config.shaderMod = true
        end
    
    }

end
