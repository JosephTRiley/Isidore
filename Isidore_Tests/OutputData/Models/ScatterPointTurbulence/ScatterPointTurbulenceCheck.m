% Data pushed by ProceduralTextureTest.cs
mkBold(1)
% close all; % So I can run it from MatLab

matDir = 'Outputs_SPT';
figDir = matDir;
vidDir = matDir;
dircheck({matDir, figDir, vidDir});

%% Plotting
tstr = {'Scatter Point Turbulence, Variable Depth'};
slen = size(noiseVal);
len = slen(end);

% Distance loopCoherence loop
for cidx = 1:len
    % Prep work
    img = squeeze(noiseVal(:,:,:,cidx));
    crng = [min(img(:)), max(img(:))];
    figure;
    colormap('gray')
    slen = size(img);
    
    % Records video
    fileName = sprintf('TurbulencePointWFS_%u', cidx);
    vidName = fullfile(vidDir, fileName);
    vid = VideoWriter(vidName, 'MPEG-4');
    vid.FrameRate = 10;
    open(vid);
    
    % Time loop
	for idx = 1:size(img,3)
        imagesc(img(: ,:, idx)');
        caxis(crng);
        axis image;
        axis xy;

        title(sprintf('Turbulence Noise, Z-position=%0.1f,\nFrame: %u, Time: %0.02f', ...
			(idx - 1) * separation(end), idx, time(idx)));
        writeVideo(vid, getframe(gcf));
    end
    close(vid);
end

%% Saves
ffile = fullfile(matDir, 'ScatterPointTurbulenceCheck');
save(ffile, 'time', 'noiseVal');

%% Cleanup
old = cd(figDir);
RemoveWhiteBorder;
cd(old)

mkBold(0)