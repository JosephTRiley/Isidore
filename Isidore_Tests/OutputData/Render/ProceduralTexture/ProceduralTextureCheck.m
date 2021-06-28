% Data pushed by ProceduralTextureTest.cs
mkBold(1)
% close all; % So I can run it from MatLab

matDir = 'MatsProc';
figDir = 'FigsProc';
vidDir = 'VidsProc';
dircheck({matDir, figDir, vidDir});

%% Infinite plane
% Standard intersection
% tstr = {'Intersection', 'Travel', 'U Coordinate', 'V Coordinate', ...
%     'X Intersect', 'Y Intersect', 'Z Intersect', 'fBm'};
% dstr = {'intArr1', 'travelArr1', 'uArr1', 'vArr1', 'i0Arr1', 'i1Arr1', ...
%     'i2Arr1', 'textureArr1'};
% cmap = {'default', 'default', 'default', 'default', 'default', ...
%     'default', 'default', 'gray'};
tstr = {'U Coordinate', 'Kolmogorov, Transverse Motion', ...
    'Kolmogorov, In-Line Motion', 'Kolmogorov, Limited Coherence'};
dstr = {'uArr1', 'textArr1','textArr2', 'textArr3'};
cmap = {'default', 'gray', 'gray', 'gray'};
cbOn = [false, false, false, false];

for didx = 1:length(dstr)
    eval(sprintf('img=%s;',dstr{didx}));
    figure;
    colormap(cmap{didx})
    slen = size(img);
    
    % Records video
    vidName = fullfile(vidDir,dstr{didx});
    vid = VideoWriter(vidName, 'MPEG-4');
    vid.FrameRate = 5;
    open(vid);
    
    for idx = 1:slen(3)
        imagesc(pos0, pos1, img(:,:,idx));
        axis image;
        if cbOn(didx)
            colorbar
        end
        title(sprintf('%s, Time: %01.2f', tstr{didx}, time(idx)));
        xlabel('[m]')
        writeVideo(vid, getframe(gcf));
    end
    close(vid);
end

%% Plots the travel
figure;
plot(time, squeeze(travelArr2(1,1,:)))
xlabel('Time [sec]')
ylabel('Travel [m]')
ffile = fullfile(figDir, 'PlanarTravel');
printFig(ffile)

%% Saves
ffile = fullfile(matDir, 'ProceduraltextureCheck');
save(ffile, 'time', 'pos0', 'pos1', '*Arr*');

%% Cleanup
old = cd(figDir);
RemoveWhiteBorder;
cd(old)

mkBold(0)