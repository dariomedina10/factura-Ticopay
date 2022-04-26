function onComplete(p) {
        var $f = $(p);
        $f.data('locked', false);  // (3)
        abp.ui.clearBusy();
    };

    function onBegin(p) {
        
        var $f = $(p);
        if (($f.data('locked') != undefined) && ($f.data('locked')))
            $f.preventDefault();
        else {
            abp.ui.setBusy();
            $f.data('locked', true);
        }
            
    }


