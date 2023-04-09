function data()
    return {
        info = {
            minorVersion = 3,
            severityAdd = "NONE",
            severityRemove = "NONE",
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
