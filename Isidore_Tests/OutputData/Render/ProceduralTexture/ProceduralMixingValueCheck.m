% Data pushed by ProceduralTextureTest.cs
mkBold(1)
% close all; % So I can run it from MatLab

matDir = 'MatsMix';
figDir = 'FigsMix';
vidDir = 'VidsMix';
dircheck({matDir, figDir, vidDir});

%% Combines data
textArr = textArr1;
clear textArr1;
textArr(:,:,:,:,2) = textArr2;
clear textArr2;

%% Infinite plane
tstr = {'U Coordinate', 'V Coordinate', 'Screen Variable Strength'};
dstr = {'uArr', 'vArr', 'textArr'};
cmap = {'default', 'default', 'gray'};
cbOn = [false, false, false];
for pmidx = 1:length(pmStrs)
	for pidx = 1:length(piStrs)
        if pmidx == 1 && pidx == 1
            darr = 1:length(dstr);
        else
            darr = 3:length(dstr);
        end
		for didx = darr
            eval(sprintf('img=%s(:,:,:, pidx, pmidx);',dstr{didx}));
			crng = [min(img(:)), max(img(:))];

			figure;
			colormap(cmap{didx})
			slen = size(img);

			% Records video
			vidName = fullfile(vidDir, [dstr{didx}, '_', piStrs{pidx}, ...
                '_', pmStrs{pmidx}]);
			vid = VideoWriter(vidName, 'MPEG-4');
			vid.FrameRate = 10;
			open(vid);

			for idx = 1:slen(3)
				imagesc(pos0, pos1, img(: ,:, idx)');
				caxis(crng);
				axis image;
				axis xy;
				if cbOn(didx)
					colorbar
				end
				title(sprintf('%s, Interpolated %s\nMixed %s, Time: %01.2f', ...
					tstr{didx}, piStrs{pidx}, pmStrs{pmidx}, time(idx)));
				xlabel('[m]')
				writeVideo(vid, getframe(gcf));
			end
			close(vid);
		end
	end
end

%% Saves
ffile = fullfile(matDir, 'ProceduralMixValueCheck');
save(ffile, 'time', 'pos0', 'pos1', 'piStrs', '*Arr*');

%% Cleanup
old = cd(figDir);
RemoveWhiteBorder;
cd(old)

mkBold(0)