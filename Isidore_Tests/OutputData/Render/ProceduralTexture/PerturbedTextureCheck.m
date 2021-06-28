% Data pushed by ProceduralTextureTest.cs
mkBold(1)
% close all; % So I can run it from MatLab

matDir = 'MatsPert';
figDir = 'FigsPert';
vidDir = 'VidsPert';
dircheck({matDir, figDir, vidDir});

%% Perturb multiplier

perturbX = {perturbMult, perturbTime, perturbTime};

tstr = {'Perturb fBm Noise, Factor: %01.2f', ...
    'Perturb fBm Noise, Time: %01.2f', ...
	'Absolute Perturb fBm Noise, Time: %01.2f'};
dstr = {'textArr1', 'textArr2', 'textArr3'};
sstr = {'pertubMulti', 'perturbMotion', 'absPerturbMotion'};
cmap = {'gray', 'gray', 'gray'};
cbOn = [true, true true];

for didx = 1:length(dstr)
    eval(sprintf('img=%s;',dstr{didx}));
    figure;
    colormap(cmap{didx})
    slen = size(img);
    
    % Records video
    vidName = fullfile(vidDir, sstr{didx});
    vid = VideoWriter(vidName, 'MPEG-4');
    vid.FrameRate = 10;
    open(vid);
    
    for idx = 1:slen(3)
        imagesc(pos0, pos1, img(:,:,idx));
        axis image;
        if cbOn(didx)
            colorbar
        end
        title(sprintf(tstr{didx}, perturbX{didx}(idx)));
        xlabel('[m]')
        writeVideo(vid, getframe(gcf));
    end
    close(vid);
end

%% Saves
ffile = fullfile(matDir, 'ProceduraltextureCheck');
save(ffile, 'perturb*', 'pos0', 'pos1', '*Arr*');

%% Cleanup
old = cd(figDir);
RemoveWhiteBorder;
cd(old)

mkBold(0)