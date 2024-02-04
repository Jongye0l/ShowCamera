namespace ShowCamera {
    public class Values {
        public static readonly Values Korean = new Values {
            Camera = "카메라",
            Size = "크기",
            Rotate = "회전",
            Width = "가로",
            Height = "세로",
            FlipHorizontal = "좌우 반전",
            FlipVertical = "상하 반전"
        };

        public static readonly Values English = new Values {
            Camera = "Camera",
            Size = "Size",
            Rotate = "Rotate",
            Width = "Width",
            Height = "Height",
            FlipHorizontal = "Flip Horizontal",
            FlipVertical = "Flip Vertical"
        };

        public string Camera;
        public string Size;
        public string Rotate;
        public string Width;
        public string Height;
        public string FlipHorizontal;
        public string FlipVertical;
    }
}