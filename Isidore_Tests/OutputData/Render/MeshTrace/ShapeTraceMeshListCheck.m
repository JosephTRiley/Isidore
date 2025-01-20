% Data pushed by ShapeTraceMesh.cs
% inter = Intesection map
% reflect = Reflection map
% depth = Depth map
% u = U coordinates for all traces
% v = V coordinates for all traces
% x = Ray's origin in world space X-axis
% y = Ray's origin in world space Y-axis
% pos0 = Grid pixel axis 0 locations [m]
% pos1 = Grid pixel axis 1 locations [m]

% close all; % So I can run it from MatLab

%% Saves
save('ShapeTraceMeshList')

%% Plots
for idx = 1:2
    figure; 

    subplot(2,3,1)
    imagesc(pos1,pos2,inter(:,:,idx)); 
    axis image; axis xy; colorbar; 
    title('Intersection')

    subplot(2,3,2) 
    imagesc(pos1,pos2,id(:,:,idx)); 
    axis image; axis xy; colorbar; 
    title('ID') 

    subplot(2,3,3) 
    imagesc(pos1,pos2,acos(abs(cosIncImg(:,:,idx))) * ...
        180/pi.*double(inter(:,:,idx))); 
    axis image xy; colorbar; 
    title('Inc. Angle')

    subplot(2,3,4)
    d = depth(:,:,idx);
    d(d==0) = min(d(d>0));
    imagesc(pos1,pos2,d); 
%     imagesc(pos1,pos2,depth(:,:,idx)); 
    axis image xy; colorbar; 
    title('Depth')

    subplot(2,3,5)
    imagesc(pos1,pos2,u(:,:,idx)); 
    axis image xy; colorbar; 
    title('U')

    subplot(2,3,6)
    imagesc(pos1,pos2,v(:,:,idx)); 
    axis image xy; colorbar; 
    title('V')
end