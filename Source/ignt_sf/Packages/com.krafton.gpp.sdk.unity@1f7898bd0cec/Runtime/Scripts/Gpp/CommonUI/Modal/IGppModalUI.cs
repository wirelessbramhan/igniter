namespace Gpp.CommonUI.Modal
{
    public interface IGppModalUI
    {
        /// <summary>
        /// Modal 화면에 필요한 데이터를 전달 드립니다.
        /// Provides the data needed for the modal screen.
        /// </summary>
        public void SetModalData(GppModalData data);
    }
}