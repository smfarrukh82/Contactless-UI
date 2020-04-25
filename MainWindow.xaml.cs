using Microsoft.Gestures.Endpoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Microsoft.Gestures.Samples.CarGestures
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum AudioSource
        {
            Phone = 0,
            Radio,
            Media,
            USB,
            Test,
            A,
            B,
            C,
            D,
            E, 
           
        }

        public const double FrameRate = 60;
        public const double AnimationSpeed = 6;
        private DispatcherTimer _timer;

        // Source
        private AudioSource _audioSource = AudioSource.Radio;

        
        private String _levelNo = "";

        private CarGesturesManager _gesturesManager = new CarGesturesManager();

        private int CallGenerationInterval = 45 * 1000; //in msec
        private Timer _callGenerator;

        private Queue<Tuple<Uri, string>> _animationQueue = new Queue<Tuple<Uri, string>>();

        public MainWindow()
        {
            InitializeComponent();
            Initialize();

            Loaded += async (s, a) => {
                Closed += (sender, arg) => _gesturesManager.Dispose();
                _gesturesManager.StatusChanged += (sender, arg) => Dispatcher.Invoke(() => GesturesServiceStatus.Fill = new SolidColorBrush(arg.Status == EndpointStatus.Detecting ? Colors.LightGreen : Colors.LightGray));


                _gesturesManager.SelectSourcePhone += (s1, a1) => SetAudioSource(AudioSource.Phone);
                _gesturesManager.SelectSourceRadio += (s1, a1) => SetAudioSource(AudioSource.Radio);
                _gesturesManager.SelectSourceMedia += (s1, a1) => SetAudioSource(AudioSource.Media);
                _gesturesManager.SelectSourceUsb += (s1, a1) => SetAudioSource(AudioSource.USB);
                _gesturesManager.SelectSourceTest += (s1, a1) => SetAudioSource(AudioSource.Test);
                _gesturesManager.SelectSourceA += (s1, a1) => SetAudioSource(AudioSource.A);
                _gesturesManager.SelectSourceB += (s1, a1) => SetAudioSource(AudioSource.B);
                _gesturesManager.SelectSourceC += (s1, a1) => SetAudioSource(AudioSource.C);
                _gesturesManager.SelectSourceD += (s1, a1) => SetAudioSource(AudioSource.D);
                _gesturesManager.SelectSourceE += (s1, a1) => SetAudioSource(AudioSource.E);
                _gesturesManager.SelectLevel += (s1, a1) => SetAudioSource1();



                await _gesturesManager.Initialize();
            };
        }

        private void Initialize()
        {
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            _timer = new DispatcherTimer();
            _timer.Tick += OnTimer_Tick;
            _timer.Interval = TimeSpan.FromSeconds(1 / FrameRate);
            _timer.Start();
        }

        protected void OnLevel()
        {
            //txtStatus.Text = _levelNo;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

          
        }

        private static double DegToRad(double deg) => deg / 180 * Math.PI;

        private static double SinD(double deg) => Math.Sin(DegToRad(deg));

        private static double CosD(double deg) => Math.Cos(DegToRad(deg));


        private void OnTimer_Tick(object sender, EventArgs e)
        {
            

            var audioSourceCount = 5;


            var audioSourceOffset = (AudioSourcePanel.ActualWidth - AudioSourceHighlight.ActualWidth * audioSourceCount) / (audioSourceCount - 1) + AudioSourceHighlight.ActualWidth;
            var audioSouceHighlightMargin = AudioSourceHighlight.Margin;
            var audioSouceHighlightTargetLeft = (int)_audioSource * audioSourceOffset;
            if((int)_audioSource > 4)
            {
                audioSouceHighlightMargin.Bottom = -238;
                audioSouceHighlightTargetLeft = ((int)_audioSource - 5) * audioSourceOffset;
                audioSouceHighlightMargin.Left += (audioSouceHighlightTargetLeft - audioSouceHighlightMargin.Left) / AnimationSpeed;
               // audioSouceHighlightMargin.Left = 0;
            }
            else
            {
                audioSouceHighlightMargin.Bottom = 325;
                audioSouceHighlightMargin.Left += (audioSouceHighlightTargetLeft - audioSouceHighlightMargin.Left) / AnimationSpeed;

            }

            AudioSourceHighlight.Margin = audioSouceHighlightMargin;




        }

       

        public void SetAudioSource(AudioSource source) {
                                                                _audioSource = source;
                                                               // MessageBox.Show("Level 5");
                                                                //lblStatus.Content = "Pressed Level 1";
                                                               // OnLevel();
                                                       }

        public void SetAudioSource1()
        {
            //_audioSource = source;
            int level = ((int)_audioSource) + 1;
            MessageBox.Show("You Pressed Level "+level);
            
            //lblStatus.Content = "Pressed Level "+(int)_audioSource;

            // OnLevel();
        }

     
        private void OnAnimatedHelpEnded(object sender, RoutedEventArgs e)
        {
            PlayNextAnimation();
        }

        private void PlayNextAnimation()
        {
            Tuple<Uri, string> nextAnimation;
            lock (_animationQueue)
            {
                if (!_animationQueue.Any())
                {
                    animatedHelpContainer.Visibility = Visibility.Collapsed;
                    return;
                }
                nextAnimation = _animationQueue.Dequeue();
            }

            animatedHelp.Source = nextAnimation.Item1;
            animatedHelp.Position = new TimeSpan(0, 0, 1);
            animatedHelpCaption.Text = nextAnimation.Item2;
            animatedHelp.Play();
            animatedHelpContainer.Visibility = Visibility.Visible;
        }

        private void HoverMouseLeave(object sender, MouseEventArgs e)
        {
            lock (_animationQueue)
            {
                _animationQueue.Clear();
            }
            animatedHelpContainer.Visibility = Visibility.Collapsed;
            animatedHelp.Source = null;
            animatedHelpCaption.Text = string.Empty;
        }

        private void ChannelMouseEnter(object sender, MouseEventArgs e)
        {
            lock (_animationQueue)
            {
                _animationQueue.Clear();
                _animationQueue.Enqueue(new Tuple<Uri, string>(GetAnimationUri("SwitchChannel"), "Switch Channel"));
            }
            PlayNextAnimation();
        }

        private void SourcesMouseEnter(object sender, MouseEventArgs e)
        {
            lock (_animationQueue)
            {
                _animationQueue.Clear();
                _animationQueue.Enqueue(new Tuple<Uri, string>(GetAnimationUri("SwitchSource"), "Switch Source"));
            }
            PlayNextAnimation();
        }

        private void VolumeMouseEnter(object sender, MouseEventArgs e)
        {
            lock (_animationQueue)
            {
                _animationQueue.Clear();
                _animationQueue.Enqueue(new Tuple<Uri, string>(GetAnimationUri("VolumeToggleMute"), "Toggle Mute"));
                _animationQueue.Enqueue(new Tuple<Uri, string>(GetAnimationUri("VolumeUp"), "Volume Up"));
                _animationQueue.Enqueue(new Tuple<Uri, string>(GetAnimationUri("VolumeDown"), "Volume Down"));
            }
            PlayNextAnimation();
        }

        private void ACMouseEnter(object sender, MouseEventArgs e)
        {
            lock (_animationQueue)
            {
                _animationQueue.Clear();
                _animationQueue.Enqueue(new Tuple<Uri, string>(GetAnimationUri("ACUp"), "Increase Temp"));
                _animationQueue.Enqueue(new Tuple<Uri, string>(GetAnimationUri("ACDown"), "Decrease Temp"));
            }
            PlayNextAnimation();
        }

        private void IncommingCallMouseEnter(object sender, MouseEventArgs e)
        {
            lock (_animationQueue)
            {
                _animationQueue.Clear();
                _animationQueue.Enqueue(new Tuple<Uri, string>(GetAnimationUri("AnswerCall"), "Answer Call"));
                _animationQueue.Enqueue(new Tuple<Uri, string>(GetAnimationUri("DismissCall"), "Dismiss Call"));
            }
            PlayNextAnimation();
        }

        private void TalkingWithMouseEnter(object sender, MouseEventArgs e)
        {
            lock (_animationQueue)
            {
                _animationQueue.Clear();
                _animationQueue.Enqueue(new Tuple<Uri, string>(GetAnimationUri("HangUpCall"), "Hang Up Call"));
            }
            PlayNextAnimation();
        }

        private Uri GetAnimationUri(string name) => new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, $@"Resources\Animations\{name}.Animated.gif"), UriKind.Absolute);
    }
}
