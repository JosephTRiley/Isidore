% Data pushed by MarchingCubeTest.cs
% pts = point grid positions
% on = surface point that are within the sphere

% close all; % So I can run it from MatLab
saveStr = 'MarchingCube';

%% Necessary data processing
if any(F(:)==0)
    F = F + 1; % Switches from 0 (C#) to 1 (MatLab) index
end

%% Displays

% % 2D points
% tag = (pts(:,3) == 0)';
% figure
% plot(pts(on & tag,1), pts(on & tag,2), '.', ...
%     pts(~on & tag,1), pts(~on & tag,2),'.');
% legend('In','Out')
% title('Cross-section of Thresholded Points');
% axis image

% % All grid points
% figure
% plot3(pts(on,1), pts(on,2), pts(on,3),'.', ...
%     pts(~on,1), pts(~on,2), pts(~on,3),'.');
% legend('In','Out')
% title('Thresholded Points');

% All vertices
figure
plot3(V(:,1), V(:,2), V(:,3),'.', pts(onSurf,1), pts(onSurf,2), pts(onSurf,3),'.');

% Facet mesh
figure
p = patch('Vertices', V, 'Faces', F, 'FaceColor', [0,0,1], ...
    'FaceAlpha', 1.0, 'EdgeColor','None', 'FaceLighting','Phong');
%'FaceLighting','Gouraud');

view(3); % sets view to az = –37.5, el = 30
axis equal
camlight('headlight'); % light aligned with camera
% camlight('left'); % light on global left for display
% camlight('right'); % light on global right for display

%% Saves patch data as Alias Wavefront Object File
save(saveStr,'V','F','pts','on','onSurf')

fid1 = fopen([saveStr,'.obj'],'w+');% opens/creates files to write
% Writes header info
fprintf(fid1, '# MZA generated object file\n\n');
% Object File Header
fprintf(fid1, 'g %s_%-u\n',saveStr,1);
% Writes vertex
for idx = 1:size(V,1)
    fprintf(fid1, 'v %-e %-e %-e\n', V(idx,1), V(idx,2), V(idx,3));
end
% Writes facets
for idx = 1:size(F,1)
    fprintf(fid1, 'f %-u %-u %-u\n', F(idx,1), F(idx,2), F(idx,3));
end
fclose(fid1);

%% Process and cleanup
figDir = 'Figs';
saveStr = sprintf('%s\\MeshedSphere', figDir);
printFig(saveStr, true);

old = cd(figDir);
Convert2pdf;
delete *.eps
cd(old)
