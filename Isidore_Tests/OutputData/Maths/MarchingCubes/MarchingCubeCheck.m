% close all; % Shuts them down

% %% Necessary data processing
% if any(F==0)
%     F = F + 1; % Switches from 0 (C#) to 1 (MatLab) index
% end

%% Display
gIdx = double(gIdx);
plotPerFig = 16;
sq = ceil(sqrt(plotPerFig));
figCnt = 0;
for idx = 1:gIdx
    figTag = mod(idx-1,plotPerFig);
    
    % Opens new figure
    if figTag==0
        figure
        figCnt = figCnt + 1;
    end
        
    % Extracts data from array
    in = inArr{idx};
    v = vArr{idx};
    f = fArr{idx} + 1;
    
    subplot(sq,sq,figTag+1)
    % Points
    plot3(pts(:,1), pts(:,2), pts(:,3),'+')
    hold on
        % On point
        plot3(pts(in,1), pts(in,2), pts(in,3),'o')
        % Interpolated vertices
%         plot3(v(:,1), v(:,2), v(:,3),'x')
        % facets
        p = patch('Vertices', v, 'Faces', f, 'FaceColor', 'blue', ...
            'FaceAlpha', 0.5);
    hold off
    title(num2str(idx))
    set(gca,'XTickLabel',[])
    set(gca,'YTickLabel',[])
    set(gca,'ZTickLabel',[])
%     axis image
    
end

%% Process and cleanup
figDir = 'Figs';
if ~exist(figDir,'dir'); mkdir(figDir); end
for idx = 1 : figCnt
    figure(idx)
    saveStr = sprintf('%s\\MarchingCubes%02d', figDir, idx);
    printFig(saveStr, true);
end

old = cd(figDir);
Convert2pdf;
delete *.eps
cd(old)