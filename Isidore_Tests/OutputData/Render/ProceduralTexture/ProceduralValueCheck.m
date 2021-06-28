% Data pushed by ProceduralTextureTest.cs
mkBold(1)
% close all; % So I can run it from MatLab

matDir = 'MatsVal';
figDir = 'FigsVal';
vidDir = 'VidsVal';
dircheck({matDir, figDir, vidDir});

%% Infinite plane
tstr = {'U Coordinate', 'V Coordinate', 'Kolmogorov, Variable Strength'};
dstr = {'uArr', 'vArr', 'textArr'};
cmap = {'default', 'default', 'gray'};
cbOn = [false, false, false];

for pidx = 1:length(piStrs)
    for didx = 1:length(dstr)
        eval(sprintf('img=%s(:,:,:,pidx);',dstr{didx}));
        crng = [min(img(:)), max(img(:))];

        figure;
        colormap(cmap{didx})
        slen = size(img);

        % Records video
        vidName = fullfile(vidDir, [dstr{didx}, '_', piStrs{pidx}]);
        vid = VideoWriter(vidName, 'MPEG-4');
        vid.FrameRate = 5;
        open(vid);

        for idx = 1:slen(3)
            imagesc(pos0, pos1, img(: ,:, idx)');
            caxis(crng);
            axis image;
            axis xy;
            if cbOn(didx)
                colorbar
            end
            title(sprintf('%s,\nInterpolated %s, Time: %01.2f', ...
                tstr{didx}, piStrs{pidx}, time(idx)));
            xlabel('[m]')
            writeVideo(vid, getframe(gcf));
        end
        close(vid);
    end
end

%% Saves
ffile = fullfile(matDir, 'ProceduralValueCheck');
save(ffile, 'time', 'pos0', 'pos1', 'piStrs', '*Arr*');

%% Cleanup
old = cd(figDir);
RemoveWhiteBorder;
cd(old)

mkBold(0)